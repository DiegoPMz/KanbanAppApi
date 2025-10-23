using KanbanAppApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace KanbanAppApi.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        Task<string> GenerateRefreshToken(User user);
        Task<(string accessToken, string refreshToken)> CreateAuthTokens(User user);
        Task<TokenValidationResult> ValidateToken(string token);
        Task InvalidateRefreshTokenByJtiAsync(Guid tokenJti);
    }
}
