using KanbanAppApi.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;
using FluentResults;
using KanbanAppApi.Errors;
using Microsoft.IdentityModel.JsonWebTokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace KanbanAppApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenService _tokenService;
        private readonly IUserService  _userService ;

        private const string RouteCallback = "https://localhost:7144/api/auth/callback";
        private const string GoogleTokenEndpoint = "https://oauth2.googleapis.com/token";
        private const string GoogleCertsEndpoint = "https://www.googleapis.com/oauth2/v3/certs";
        public AuthService(
            IConfiguration configuration, 
            IHttpClientFactory httpClientFactory, 
            ITokenService tokenService, 
            IUserService userService
            )
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
            _userService = userService;
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

            var tokenHandler = new JsonWebTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = ["accounts.google.com", "https://accounts.google.com"],
                ValidateAudience = true,
                ValidAudience = _configuration["Google:ClientId"],
                ValidateLifetime = true,
                IssuerSigningKeys = jwks.Keys
            };

            var tokenValidationResult = await tokenHandler.ValidateTokenAsync(idToken, validationParameters);
            return MapGoogleClaims(tokenValidationResult.ClaimsIdentity);
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

        public async Task<Result<(string AccessToken, string RefreshToken)>> RefreshAsync(string refreshTokenCookie)
        {
            var tokenValidation = await _tokenService.ValidateToken(refreshTokenCookie);
            if (!tokenValidation.IsValid) return AuthErrors.InvalidRefreshToken();
            
            var jtiClaim = tokenValidation.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (jtiClaim is null || !Guid.TryParse(jtiClaim, out var tokenJti))
                return AuthErrors.InvalidRefreshToken();

            await _tokenService.InvalidateRefreshTokenByJtiAsync(tokenJti);

            var userIdClaim = tokenValidation.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId)) return AuthErrors.InvalidRefreshToken();

            var user = await _userService.GetByIdAsync(userId);
            if (user.IsFailed) return AuthErrors.InvalidRefreshToken();

            return await _tokenService.CreateAuthTokens(user.Value);
        }

        private static GoogleIdTokenClaims MapGoogleClaims(ClaimsIdentity claims) => new()
        {
            Sub = claims.FindFirst("sub")?.Value ?? "",
            Email = claims.FindFirst("email")?.Value ?? "",
            EmailVerified = bool.TryParse(claims.FindFirst("email_verified")?.Value, out var verified) ? verified : null,
            Name = claims.FindFirst("name")?.Value,
            Picture = claims.FindFirst("picture")?.Value,
            Exp = long.Parse(claims.FindFirst("exp")?.Value ?? "0"),
            Iat = long.Parse(claims.FindFirst("iat")?.Value ?? "0"),
            Aud = claims.FindFirst("aud")?.Value ?? "",
            Iss = claims.FindFirst("iss")?.Value ?? "",
            GivenName = claims.FindFirst("given_name")?.Value,
        };
    }
}
