using Microsoft.AspNetCore.Mvc;
using ShelfWise.Api.Models;

namespace ShelfWise.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;

        public BooksController(ILogger<BooksController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Book>> Get()
        {
            // placeholder - return empty list for initial scaffold
            return Ok(new List<Book>());
        }
    }
}
