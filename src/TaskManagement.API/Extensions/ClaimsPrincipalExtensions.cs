using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManagement.Application.Common.Constants;
using TaskManagement.Application.Common.Exceptions;

namespace TaskManagement.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        // Varsayılan claim eşlemesine göre "sub", NameIdentifier'a dönüşür; ikisini de destekliyoruz
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedException(ErrorMessages.InvalidToken);
    }
}