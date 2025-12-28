using System.Security.Claims;

namespace Cinema_MGMT.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email, List<string>? roles = null);
    ClaimsPrincipal? ValidateToken(string token);
}

