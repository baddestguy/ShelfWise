using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelfWise.Domain.Models;
using ShelfWise.Api.Models;
using ShelfWise.Repository.Data;

namespace ShelfWise.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(Policy = "LibrarianOrAdmin")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll(CancellationToken ct)
        {
            var users = await _db.Users
                .AsNoTracking()
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync(ct);

            return Ok(users.Select(UserResponseDto.FromUser));
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "LibrarianOrAdmin")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id, CancellationToken ct)
        {
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, ct);

            if (user == null) return NotFound();

            return Ok(UserResponseDto.FromUser(user));
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken ct)
        {
            var firstName = dto.FirstName.Trim();
            var lastName = dto.LastName.Trim();

            if (firstName.Length == 0 || lastName.Length == 0)
            {
                return BadRequest(new { message = "First name and last name are required." });
            }

            var user = new User { FirstName = firstName, LastName = lastName };
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, UserResponseDto.FromUser(user));
        }
    }
}
