using Divider.DTOs.Auth;

namespace Divider.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterUserDto request);

        Task<AuthResponseDto?> LoginAsync(LoginDto request);
    }
}
