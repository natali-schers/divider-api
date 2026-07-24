using Divider.Data;
using Divider.DTOs.Auth;
using Divider.Models;
using Divider.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Divider.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly DividerDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(DividerDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterUserDto request)
    {
        var emailNormalized = request.Email.Trim().ToLowerInvariant();

        var emailInUse = await _context.Users
            .AnyAsync(u => u.Email == emailNormalized);

        if (emailInUse)
        {
            return Conflict("Já existe uma conta com esse email.");
        }

        if (request.Password.Length < 6)
        {
            return BadRequest("A senha deve ter ao menos 6 caracteres.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = emailNormalized,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var dto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
        };

        var token = _jwtService.GenerateToken(user);
        return CreatedAtAction(nameof(Register), new AuthResponseDto { Token = token, User = dto });
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto request)
    {
        var emailNormalized = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == emailNormalized);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Email ou senha inválidos.");
        }

        var dto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
        };

        var token = _jwtService.GenerateToken(user);
        return Ok(new AuthResponseDto { Token = token, User = dto });
    }
}