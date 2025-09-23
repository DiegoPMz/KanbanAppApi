using KanbanAppApi.Models;
using System.Security.Claims;

namespace KanbanAppApi.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
