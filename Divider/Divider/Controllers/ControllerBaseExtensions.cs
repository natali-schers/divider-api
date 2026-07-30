using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace Divider.Controllers;

public static class ControllerBaseExtensions
{
    public static Guid GetCurrentUserId(this ControllerBase controller)
    {
        var userIdClaim = controller.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Token inválido: ID de usuário não encontrado.");
        }

        return userId;
    }

    public static string GetCurrentUserEmail(this ControllerBase controller)
    {
        var emailClaim = controller.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

        if (emailClaim is null)
        {
            throw new UnauthorizedAccessException("Token inválido: Email de usuário não encontrado.");
        }

        return emailClaim;
    }
}