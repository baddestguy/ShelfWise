using ShelfWise.Api.Models;

namespace ShelfWise.Api.Services
{
    public interface IAiBookSearchService
    {
        Task<AiBookSearchResponseDto> SearchAsync(string query, CancellationToken ct = default);
    }
}
