using Microsoft.AspNetCore.Mvc;
using ShelfWise.Api.Models;
using ShelfWise.Api.Services;

namespace ShelfWise.Api.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        private readonly IAiBookSearchService _aiBookSearchService;

        public AiController(IAiBookSearchService aiBookSearchService)
        {
            _aiBookSearchService = aiBookSearchService;
        }

        [HttpPost("book-search")]
        public async Task<ActionResult<AiBookSearchResponseDto>> SearchBooks(
            [FromBody] AiBookSearchRequestDto dto,
            CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _aiBookSearchService.SearchAsync(dto.Query, ct);
            return Ok(response);
        }
    }
}
