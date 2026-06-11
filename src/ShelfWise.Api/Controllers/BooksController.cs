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
            var all = await _service.GetAllAsync(ct);
            var book = all.FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();
            return Ok(book);
        }
    }
}
