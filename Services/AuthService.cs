using KanbanAppApi.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace KanbanAppApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        private const string RouteCallback = "https://localhost:7144/api/auth/callback";
        private const string GoogleTokenEndpoint = "https://oauth2.googleapis.com/token";
        private const string GoogleCertsEndpoint = "https://www.googleapis.com/oauth2/v3/certs";
        public AuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory )
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<GoogleTokenResponse> ExchangeCodeForTokenAsync(string code, string codeVerifier)
        {
            var googleParameters = new Dictionary<string, string?>
            {
                ["client_id"] = _configuration["Google:ClientId"],
                ["code"] = code,
                ["redirect_uri"] = RouteCallback,
                ["code_verifier"] = codeVerifier,
                ["grant_type"] = "authorization_code",
                ["client_secret"] = _configuration["Google:ClientSecret"] 
            };

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync(
                GoogleTokenEndpoint,
                new FormUrlEncodedContent(googleParameters)
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"<----------- Error: {error}");
                throw new Exception("Failed to GET Google tokens.");
            }


            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(responseBody) ?? throw new Exception("Failed to deserialize Google token response.");
            return tokenResponse;
        }

        public async Task<GoogleIdTokenClaims> ValidateGoogleIdTokenAsync(string idToken)
        {
            JsonWebKeySet jwks = await _httpClientFactory
              .CreateClient()
              .GetFromJsonAsync<JsonWebKeySet>(GoogleCertsEndpoint) ?? throw new Exception("No se pudieron obtener las claves públicas de Google.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "https://accounts.google.com",
                ValidateAudience = true,
                ValidAudience = _configuration["Google:ClientId"],
                ValidateLifetime = true,
                IssuerSigningKeys = jwks.Keys
            };

            ClaimsPrincipal principal = tokenHandler.ValidateToken(idToken, validationParameters, out var _);
            return MapGoogleClaims(principal);
        }

        public (string GoogleUrl, string CodeVerifier) BuildGoogleLoginUrl()
        {
            var (codeChallenge, verifier) = Utilities.PKCEUtil.Generate();
            var googleQuery = new Dictionary<string, string?>
            {
                ["client_id"] = _configuration["Google:ClientId"],
                ["response_type"] = "code",
                ["scope"] = "openid email profile",
                ["redirect_uri"] = RouteCallback,
                ["code_challenge"] = codeChallenge,
                ["code_challenge_method"] = "S256",
                ["prompt"] = "consent"
            };

            var googleUrl = QueryHelpers.AddQueryString(
                "https://accounts.google.com/o/oauth2/v2/auth",
                googleQuery
            );
            return (googleUrl, verifier);
        }

        private static GoogleIdTokenClaims MapGoogleClaims(ClaimsPrincipal principal) => new()
        {
            Sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "",
            Email = principal.FindFirst(ClaimTypes.Email)?.Value ?? "",
            EmailVerified = bool.TryParse(principal.FindFirst("email_verified")?.Value, out var verified) ? verified : null,
            Name = principal.FindFirst("name")?.Value,
            Picture = principal.FindFirst("picture")?.Value,
            Exp = long.Parse(principal.FindFirst("exp")?.Value ?? "0"),
            Iat = long.Parse(principal.FindFirst("iat")?.Value ?? "0"),
            Aud = principal.FindFirst("aud")?.Value ?? "",
            Iss = principal.FindFirst("iss")?.Value ?? "",
            GivenName = principal.FindFirst(ClaimTypes.GivenName)?.Value,
        };
    }
}
