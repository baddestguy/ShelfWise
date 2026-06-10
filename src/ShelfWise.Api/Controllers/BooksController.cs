using Microsoft.AspNetCore.Mvc;
using ShelfWise.Domain.Models;
using ShelfWise.Services.Interfaces;

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
    }
}
