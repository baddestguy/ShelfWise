using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelfWise.Api.Models;
using ShelfWise.Api.Services;
using ShelfWise.Domain.Models;
using ShelfWise.Repository.Data;

namespace ShelfWise.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthController(AppDbContext db, IJwtTokenService jwtTokenService)
        {
            _db = db;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
        {
            var username = dto.Username.Trim().ToLowerInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username, ct);
            if (user == null || string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            return Ok(new LoginResponseDto
            {
                Token = _jwtTokenService.CreateToken(user),
                User = UserResponseDto.FromUser(user)
            });
        }
    }
}
