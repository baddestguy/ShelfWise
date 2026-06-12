using Microsoft.Extensions.Caching.Memory;
using ShelfWise.Domain.Models;
using ShelfWise.Repository.Repositories;

namespace ShelfWise.Services.Services
{
    public class BookService : IBookService
    {
        private static readonly HashSet<string> InventoryCacheKeys = new();
        private static readonly object InventoryCacheLock = new();
        private static readonly MemoryCacheEntryOptions CacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };

        private readonly IMemoryCache _cache;
        private readonly IBookRepository _repo;

        public BookService(IBookRepository repo, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken ct = default)
        {
            return await _repo.GetAllAsync(ct);
        }

        public async Task<IEnumerable<BookInventoryItem>> SearchInventoryAsync(string? search, CancellationToken ct = default)
        {
            var key = $"books:inventory:search:{NormalizeCacheSegment(search)}";
            if (_cache.TryGetValue(key, out IReadOnlyList<BookInventoryItem>? cached) && cached != null)
            {
                return cached;
            }

            var items = (await _repo.SearchInventoryAsync(search, ct)).ToList();
            SetInventoryCache(key, items);
            return items;
        }

        public async Task<BookInventoryItem?> GetInventoryByIdAsync(int id, CancellationToken ct = default)
        {
            var key = $"books:inventory:id:{id}";
            if (_cache.TryGetValue(key, out BookInventoryItem? cached) && cached != null)
            {
                return cached;
            }

            var item = await _repo.GetInventoryByIdAsync(id, ct);
            if (item != null)
            {
                SetInventoryCache(key, item);
            }

            return item;
        }

        public async Task<Book> CreateAsync(Book book, CancellationToken ct = default)
        {
            // Ensure basic invariants: TotalCopies must be non-negative
            if (book.TotalCopies < 0) book.TotalCopies = 0;

            var created = await _repo.AddAsync(book, ct);
            ClearInventoryCache();
            return created;
        }

        public async Task<bool> UpdateAsync(int id, Book toUpdate, CancellationToken ct = default)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return false;

            // map allowed updatable fields
            existing.Title = toUpdate.Title;
            existing.Author = toUpdate.Author;
            existing.Genre = toUpdate.Genre;
            existing.Category = toUpdate.Category;
            existing.TotalCopies = toUpdate.TotalCopies;

            // If TotalCopies decreased below zero, clamp
            if (existing.TotalCopies < 0) existing.TotalCopies = 0;

            var updated = await _repo.UpdateAsync(existing, ct);
            if (updated) ClearInventoryCache();
            return updated;
        }

        public async Task<Book?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(id, ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _repo.DeleteAsync(id, ct);
            if (deleted) ClearInventoryCache();
            return deleted;
        }

        public async Task<bool> CheckOutAsync(int bookId, int userId, int dueDays = 14, CancellationToken ct = default)
        {
            var activeBorrow = await _repo.GetActiveBorrowRecordAsync(bookId, userId, ct);
            if (activeBorrow != null)
            {
                return false;
            }

            var total = await _repo.GetTotalCopiesAsync(bookId, ct);
            var checkedOutCount = await _repo.GetCheckedOutCountAsync(bookId, ct);
            if (checkedOutCount >= total)
            {
                // no copies available
                return false;
            }

            var dueAt = DateTime.UtcNow.AddDays(dueDays);
            var checkedOut = await _repo.TryCheckoutAsync(bookId, userId, dueAt, ct);
            if (checkedOut) ClearInventoryCache();
            return checkedOut;
        }

        public async Task<bool> CheckInAsync(int bookId, int userId, CancellationToken ct = default)
        {
            var checkedIn = await _repo.TryCheckinAsync(bookId, userId, ct);
            if (checkedIn) ClearInventoryCache();
            return checkedIn;
        }

        public async Task<int> PlaceHoldAsync(int bookId, int userId, CancellationToken ct = default)
        {
            var holdId = await _repo.CreateHoldAsync(bookId, userId, ct);
            ClearInventoryCache();
            return holdId;
        }

        private static string NormalizeCacheSegment(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "all" : value.Trim().ToLowerInvariant();
        }

        private void SetInventoryCache<T>(string key, T value)
        {
            _cache.Set(key, value, CacheOptions);

            lock (InventoryCacheLock)
            {
                InventoryCacheKeys.Add(key);
            }
        }

        private void ClearInventoryCache()
        {
            string[] keys;
            lock (InventoryCacheLock)
            {
                keys = InventoryCacheKeys.ToArray();
                InventoryCacheKeys.Clear();
            }

            foreach (var key in keys)
            {
                _cache.Remove(key);
            }
        }
    }
}
