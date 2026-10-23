using Divider.Models;

namespace Divider.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
