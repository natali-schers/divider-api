using Divider.DTOs.Auth;
using Divider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Divider.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterUserDto request)
        {
            var result = await _authService.RegisterAsync(request);
            return result is null ? Conflict("Já existe uma conta com esse email.") : Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto request)
        {
            var result = await _authService.LoginAsync(request);
            return result is null ? Unauthorized("Email ou senha inválidos.") : Ok(result);
        }
    }
}