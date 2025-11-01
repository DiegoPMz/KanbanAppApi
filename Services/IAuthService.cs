using KanbanAppApi.Models;
using FluentResults;

namespace KanbanAppApi.Services
{
    public interface IAuthService
    {
        Task<GoogleTokenResponse> ExchangeCodeForTokenAsync(string code, string codeVerifier);
        Task<GoogleIdTokenClaims> ValidateGoogleIdTokenAsync(string idToken);
        (string GoogleUrl, string CodeVerifier) BuildGoogleLoginUrl();
        Task<Result<(string AccessToken, string RefreshToken)>> RefreshAsync(string refreshTokenCookie);

    }
}
