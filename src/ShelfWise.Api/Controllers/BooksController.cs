using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShelfWise.Api.Models;
using ShelfWise.Domain.Models;
using ShelfWise.Services.Services;

namespace ShelfWise.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IBookService _service;

        public BooksController(ILogger<BooksController> logger, IBookService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("{id}/checkout")]
        [Authorize(Policy = "LibrarianOrAdmin")]
        public async Task<IActionResult> Checkout(int id, [FromBody] CheckoutRequestDto dto, CancellationToken ct)
        {
            var ok = await _service.CheckOutAsync(id, dto.UserId, dto.DueDays, ct);
            if (!ok) return BadRequest(new { message = "No copies available, or this user already has this book checked out." });
            return Ok();
        }

        [HttpPost("{id}/checkin")]
        [Authorize(Policy = "LibrarianOrAdmin")]
        public async Task<IActionResult> Checkin(int id, [FromBody] CheckinRequestDto dto, CancellationToken ct)
        {
            var ok = await _service.CheckInAsync(id, dto.UserId, ct);
            if (!ok) return BadRequest(new { message = "Not checked out by user" });
            return Ok();
        }

        [HttpPost("{id}/hold")]
        [Authorize(Policy = "LibrarianOrAdmin")]
        public async Task<IActionResult> PlaceHold(int id, [FromBody] HoldRequestDto dto, CancellationToken ct)
        {
            var holdId = await _service.PlaceHoldAsync(id, dto.UserId, ct);
            return CreatedAtAction(nameof(GetById), new { id }, new { holdId });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookResponseDto>>> Get([FromQuery] string? search, CancellationToken ct)
        {
            var books = await _service.SearchInventoryAsync(search, ct);
            return Ok(books.Select(BookResponseDto.FromInventory));
        }

        [HttpPost]
        [Authorize(Policy = "LibrarianOrAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateBookDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var domainBook = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Genre = dto.Genre,
                TotalCopies = dto.TotalCopies
            };

            if (Enum.TryParse<Category>(dto.Category, true, out var parsed))
            {
                domainBook.Category = parsed;
            }
            else
            {
                domainBook.Category = Category.NonFiction;
            }

            var created = await _service.CreateAsync(domainBook, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookResponseDto>> GetById(int id, CancellationToken ct)
        {
            var book = await _service.GetInventoryByIdAsync(id, ct);
            if (book == null) return NotFound();

            return Ok(BookResponseDto.FromInventory(book));
        }

        [HttpPatch("{id:int}")]
        [Authorize(Policy = "LibrarianOrAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _service.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            // merge only provided fields
            if (dto.Title != null) existing.Title = dto.Title;
            if (dto.Author != null) existing.Author = dto.Author;
            if (dto.Genre != null) existing.Genre = dto.Genre;
            if (dto.TotalCopies.HasValue) existing.TotalCopies = dto.TotalCopies.Value;

            if (!string.IsNullOrWhiteSpace(dto.Category) && Enum.TryParse<Category>(dto.Category, true, out var parsed))
            {
                existing.Category = parsed;
            }

            var ok = await _service.UpdateAsync(id, existing, ct);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
