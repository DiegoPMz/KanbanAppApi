using KanbanAppApi.Models;
using System.Security.Claims;

namespace KanbanAppApi.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        Task<string> GenerateRefreshToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
        Task InvalidateRefreshTokenByJtiAsync(Guid tokenJti);
    }
}
