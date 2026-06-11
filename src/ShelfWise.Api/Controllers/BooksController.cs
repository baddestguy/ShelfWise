using Microsoft.AspNetCore.Mvc;
using ShelfWise.Domain.Models;
using ShelfWise.Services.Interfaces;
using ShelfWise.Api.Models;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> Get(CancellationToken ct)
        {
            var books = await _service.GetAllAsync(ct);
            return Ok(books);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var domainBook = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Genre = dto.Genre,
                TotalCopies = dto.TotalCopies,
                OnHold = 0,
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
        public async Task<ActionResult<Book>> GetById(int id, CancellationToken ct)
        {
            var book = await _service.GetByIdAsync(id, ct);
            if (book == null) return NotFound();

            return Ok(book);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _service.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            // merge only provided fields
            if (dto.Title != null) existing.Title = dto.Title;
            if (dto.Author != null) existing.Author = dto.Author;
            if (dto.Genre != null) existing.Genre = dto.Genre;
            if (dto.OnHold.HasValue) existing.OnHold = dto.OnHold.Value;
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
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
