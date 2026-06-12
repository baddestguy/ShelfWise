using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using ShelfWise.Api.Models;
using ShelfWise.Domain.Models;
using ShelfWise.Services.Services;

namespace ShelfWise.Api.Services
{
    public class AiBookSearchService : IAiBookSearchService
    {
        private const string EmbeddingsEndpoint = "https://api.openai.com/v1/embeddings";
        private readonly IBookService _bookService;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AiBookSearchService> _logger;

        public AiBookSearchService(
            IBookService bookService,
            IMemoryCache cache,
            IConfiguration configuration,
            HttpClient httpClient,
            ILogger<AiBookSearchService> logger)
        {
            _bookService = bookService;
            _cache = cache;
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<AiBookSearchResponseDto> SearchAsync(string query, CancellationToken ct = default)
        {
            var trimmedQuery = query.Trim();
            var books = (await _bookService.SearchInventoryAsync(null, ct)).ToList();
            if (books.Count == 0)
            {
                return new AiBookSearchResponseDto
                {
                    Query = trimmedQuery,
                    Mode = "empty-catalog",
                    Summary = "No books are available to search."
                };
            }

            var apiKey = _configuration["OPENAI_API_KEY"];
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                try
                {
                    return await SearchWithEmbeddingsAsync(trimmedQuery, books, apiKey, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "AI semantic search failed; using fallback keyword scoring.");
                }
            }

            return SearchWithFallback(trimmedQuery, books, string.IsNullOrWhiteSpace(apiKey) ? "fallback-no-api-key" : "fallback-ai-error");
        }

        private async Task<AiBookSearchResponseDto> SearchWithEmbeddingsAsync(
            string query,
            IReadOnlyCollection<BookInventoryItem> books,
            string apiKey,
            CancellationToken ct)
        {
            var queryEmbedding = await CreateEmbeddingAsync(query, apiKey, ct);
            var matches = new List<AiBookMatchDto>();

            foreach (var book in books)
            {
                var bookEmbedding = await GetBookEmbeddingAsync(book, apiKey, ct);
                var semanticScore = CosineSimilarity(queryEmbedding, bookEmbedding);
                var availabilityBoost = book.AvailableCopies > 0 ? 0.04 : 0;
                var score = Math.Min(1, semanticScore + availabilityBoost);

                matches.Add(new AiBookMatchDto
                {
                    Book = BookResponseDto.FromInventory(book),
                    Score = Math.Round(score, 3),
                    Reason = BuildSemanticReason(book, semanticScore)
                });
            }

            return new AiBookSearchResponseDto
            {
                Query = query,
                Mode = "openai-embeddings",
                Summary = "Ranked with OpenAI embeddings over local catalog metadata.",
                Matches = matches
                    .OrderByDescending(match => match.Score)
                    .ThenBy(match => match.Book.Title)
                    .Take(5)
                    .ToList()
            };
        }

        private async Task<float[]> GetBookEmbeddingAsync(BookInventoryItem book, string apiKey, CancellationToken ct)
        {
            var cacheKey = $"ai:book-embedding:{book.Id}:{book.Title}:{book.Author}:{book.Category}:{book.Genre}:{book.TotalCopies}";
            if (_cache.TryGetValue(cacheKey, out float[]? cached) && cached != null)
            {
                return cached;
            }

            var embedding = await CreateEmbeddingAsync(BuildBookText(book), apiKey, ct);
            _cache.Set(cacheKey, embedding, TimeSpan.FromHours(12));
            return embedding;
        }

        private async Task<float[]> CreateEmbeddingAsync(string input, string apiKey, CancellationToken ct)
        {
            var model = _configuration["OPENAI_EMBEDDING_MODEL"];
            if (string.IsNullOrWhiteSpace(model))
            {
                model = "text-embedding-3-small";
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, EmbeddingsEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = new StringContent(
                JsonSerializer.Serialize(new { model, input }),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
            return document.RootElement
                .GetProperty("data")[0]
                .GetProperty("embedding")
                .EnumerateArray()
                .Select(value => value.GetSingle())
                .ToArray();
        }

        private static AiBookSearchResponseDto SearchWithFallback(
            string query,
            IReadOnlyCollection<BookInventoryItem> books,
            string mode)
        {
            var queryTerms = Tokenize(query).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var matches = books
                .Select(book =>
                {
                    var bookTerms = Tokenize(BuildBookText(book)).ToHashSet(StringComparer.OrdinalIgnoreCase);
                    var overlap = queryTerms.Count == 0 ? 0 : queryTerms.Count(term => bookTerms.Contains(term));
                    var score = queryTerms.Count == 0 ? 0 : (double)overlap / queryTerms.Count;
                    if (book.AvailableCopies > 0) score += 0.05;

                    return new AiBookMatchDto
                    {
                        Book = BookResponseDto.FromInventory(book),
                        Score = Math.Round(Math.Min(1, score), 3),
                        Reason = overlap > 0
                            ? $"Matched {overlap} catalog keyword{(overlap == 1 ? "" : "s")} from the request."
                            : "Low keyword overlap; included as a catalog fallback result."
                    };
                })
                .OrderByDescending(match => match.Score)
                .ThenBy(match => match.Book.Title)
                .Take(5)
                .ToList();

            return new AiBookSearchResponseDto
            {
                Query = query,
                Mode = mode,
                Summary = mode == "fallback-no-api-key"
                    ? "OpenAI API key not configured. Used local keyword scoring fallback."
                    : "OpenAI semantic search failed. Used local keyword scoring fallback.",
                Matches = matches
            };
        }

        private static string BuildBookText(BookInventoryItem book)
        {
            return $"{book.Title}. Author: {book.Author}. Category: {book.Category}. Genre: {book.Genre}. Total copies: {book.TotalCopies}. Available copies: {book.AvailableCopies}.";
        }

        private static string BuildSemanticReason(BookInventoryItem book, double semanticScore)
        {
            var availability = book.AvailableCopies > 0 ? " It is currently available." : " All copies are currently checked out.";
            return $"Semantic similarity score {semanticScore:0.000} based on title, author, category, genre, and availability metadata.{availability}";
        }

        private static IEnumerable<string> Tokenize(string value)
        {
            return value
                .ToLowerInvariant()
                .Split(
                    new[] { ' ', '.', ',', ';', ':', '-', '_', '/', '\\', '(', ')', '[', ']', '"', '\'' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Where(term => term.Length > 2);
        }

        private static double CosineSimilarity(IReadOnlyList<float> left, IReadOnlyList<float> right)
        {
            double dot = 0;
            double leftMagnitude = 0;
            double rightMagnitude = 0;

            for (var i = 0; i < left.Count && i < right.Count; i++)
            {
                dot += left[i] * right[i];
                leftMagnitude += left[i] * left[i];
                rightMagnitude += right[i] * right[i];
            }

            if (leftMagnitude == 0 || rightMagnitude == 0) return 0;
            return dot / (Math.Sqrt(leftMagnitude) * Math.Sqrt(rightMagnitude));
        }
    }
}
