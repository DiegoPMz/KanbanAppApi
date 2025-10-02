using KanbanAppApi.Filters;
using KanbanAppApi.Models;
using KanbanAppApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace KanbanAppApi.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private const string _CodeVerifierCookieName = "code_verifier";
        private const string _AccessTokenCookie= "access_token";
        private const string _RefreshTokenCookie = "refresh_token";

        public AuthController(IAuthService authService, IUserService userService, ITokenService tokenService)
        {
            _authService = authService;
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpGet("login")]
        public void Login()
        {
            var (GoogleUrl, CodeVerifier) = _authService.BuildGoogleLoginUrl();

            HttpContext.Response.Cookies.Append(_CodeVerifierCookieName, CodeVerifier, new CookieOptions
            {
                SameSite = SameSiteMode.Lax,
                Secure = true,
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(5)
            });

            HttpContext.Response.Redirect(GoogleUrl);
        }

        [HttpGet("callback")]
        public async Task<IResult> Callback([FromQuery] string code)
        {
            HttpContext.Request.Cookies.TryGetValue(_CodeVerifierCookieName, out var codeVerifier);
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(codeVerifier)) return TypedResults.Unauthorized();

            GoogleTokenResponse tokens = await _authService.ExchangeCodeForTokenAsync(code, codeVerifier);
            if (tokens is null) return TypedResults.Unauthorized();

            HttpContext.Response.Cookies.Delete(_CodeVerifierCookieName);
            GoogleIdTokenClaims userClaims = await _authService.ValidateGoogleIdTokenAsync(tokens.id_token);

            User? userDb = await _userService.GetUserBySubAsync(userClaims.Sub);

            if (userDb is null)
            {
                User? createdUser = await _userService.CreateUserFromSubAsync(userClaims.Sub, userClaims.Email);
                if (createdUser is not null) await SetAuthCookies(createdUser);
            } 
            else 
            {
                await SetAuthCookies(userDb);
            }

            return Results.Redirect("http://localhost:5173");
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<NoContent> Logout()
        {
            HttpContext.Response.Cookies.Delete(_AccessTokenCookie);
            HttpContext.Response.Cookies.Delete(_RefreshTokenCookie);

            var tokenJtiClaim = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)!;

            if (Guid.TryParse(tokenJtiClaim.Value, out var jti)) await _tokenService.InvalidateRefreshTokenByJtiAsync(jti);
            return TypedResults.NoContent();
        }

        [Authorize]
        [RequireUserId]
        [HttpPost("refresh")]
        public async Task<IResult> Refresh()
        {
            HttpContext.Response.Cookies.Delete(_AccessTokenCookie);
            HttpContext.Response.Cookies.Delete(_RefreshTokenCookie);

            var userId = (Guid)HttpContext.Items["UserId"]!;
            User? user = await  _userService.GetUserDetailsByIdAsync(userId);

            if (user is null) return TypedResults.Unauthorized();

            var tokenJtiClaim = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);

            if (tokenJtiClaim is null || !Guid.TryParse(tokenJtiClaim.Value, out var jti))
                return TypedResults.Unauthorized();

            await _tokenService.InvalidateRefreshTokenByJtiAsync(jti);

            await SetAuthCookies(user);
            return TypedResults.Ok();
        }


        private async Task SetAuthCookies(User user)
        {
            var accessTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            };
            var refreshTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            };

            var accessToken = _tokenService.GenerateToken(user);
            var refreshToken = await _tokenService.GenerateRefreshToken(user);

            HttpContext.Response.Cookies.Append(_AccessTokenCookie, accessToken, accessTokenCookieOptions);
            HttpContext.Response.Cookies.Append(_RefreshTokenCookie, refreshToken, refreshTokenCookieOptions);
        }
    }
}
