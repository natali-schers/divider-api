using Divider.Data;
using Divider.DTOs.Auth;
using Divider.Models;
using Divider.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Divider.Services
{
    public class AuthService : IAuthService
    {
        private readonly DividerDbContext _context;

        private readonly IJwtService _jwtService;

        public AuthService(DividerDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterUserDto request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            if (await _context.Users.AnyAsync(u => u.Email == email))
                return null;

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return BuildResponse(user);
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            return BuildResponse(user);
        }

        private AuthResponseDto BuildResponse(User user) => new()
        {
            Token = _jwtService.GenerateToken(user),
            User = new UserDto { Id = user.Id, Name = user.Name, Email = user.Email },
        };
    }
}
