using KanbanAppApi.Models;
using System.Security.Claims;

namespace KanbanAppApi.Services
{
    public interface IAuthService
    {
        Task<GoogleTokenResponse> ExchangeCodeForTokenAsync(string code, string codeVerifier);
        Task<GoogleIdTokenClaims> ValidateGoogleIdTokenAsync(string idToken);
        (string GoogleUrl, string CodeVerifier) BuildGoogleLoginUrl();
    }
}
