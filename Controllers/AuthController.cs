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

        public AuthController(IAuthService authService, IUserService userService, ITokenService tokenService)
        {
            _authService = authService;
            _userService = userService;
            _tokenService = tokenService;
        }

        private const string CodeVerifierCookieName = "code_verifier";
        private const string AccessTokenCookie= "access_token";
        private const string RefreshTokenCookie = "refresh_token";

        [HttpGet("login")]
        public void Login()
        {
            var (googleUrl, codeVerifier) = _authService.BuildGoogleLoginUrl();

            HttpContext.Response.Cookies.Append(CodeVerifierCookieName, codeVerifier, new CookieOptions
            {
                SameSite = SameSiteMode.Lax,
                Secure = true,
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(5)
            });

            HttpContext.Response.Redirect(googleUrl);
        }

        [HttpGet("callback")]
        public async Task<IResult> Callback([FromQuery] string code)
        {
            HttpContext.Request.Cookies.TryGetValue(CodeVerifierCookieName, out var codeVerifier);
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(codeVerifier)) return TypedResults.Unauthorized();

            GoogleTokenResponse tokens = await _authService.ExchangeCodeForTokenAsync(code, codeVerifier);
            if (string.IsNullOrEmpty(tokens.id_token)) return TypedResults.Unauthorized();

            HttpContext.Response.Cookies.Delete(CodeVerifierCookieName);
            GoogleIdTokenClaims userClaims = await _authService.ValidateGoogleIdTokenAsync(tokens.id_token);

            var existingUser = await _userService.GetUserBySubAsync(userClaims.Sub);
            
            if (existingUser is null)
            {
                var createdUser = await _userService.CreateUserFromSubAsync(userClaims.Sub, userClaims.Email);
                if (createdUser is null) return TypedResults.Unauthorized();
                
                var (accessToken,refreshToken) = await _tokenService.CreateAuthTokens(createdUser);
                SetAuthCookies(accessToken, refreshToken);
            } 
            else 
            {
                var (accessToken,refreshToken) = await _tokenService.CreateAuthTokens(existingUser);
                SetAuthCookies(accessToken, refreshToken);
            }
            
            return Results.Redirect("http://localhost:5173");
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<NoContent> Logout()
        {
            HttpContext.Response.Cookies.Delete(AccessTokenCookie);
            HttpContext.Response.Cookies.Delete(RefreshTokenCookie);

            var tokenJtiClaim = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)!;

            if (Guid.TryParse(tokenJtiClaim.Value, out var jti)) await _tokenService.InvalidateRefreshTokenByJtiAsync(jti);
            return TypedResults.NoContent();
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh()
        {
            var refreshTokenCookie = HttpContext.Request.Cookies[RefreshTokenCookie];
            if (string.IsNullOrEmpty(refreshTokenCookie)) return Unauthorized();

            var tokenValidation = await _tokenService.ValidateToken(refreshTokenCookie);
            if (!tokenValidation.IsValid)
            {
                HttpContext.Response.Cookies.Delete(RefreshTokenCookie);
                return Unauthorized();
            }

            var tokenJtiClaim = tokenValidation.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (tokenJtiClaim is null || !Guid.TryParse(tokenJtiClaim, out var tokenJti)) return Unauthorized();
            
            await _tokenService.InvalidateRefreshTokenByJtiAsync(tokenJti);
           
            var userIdClaim = tokenValidation.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId )) return Unauthorized();

            var user = await _userService.GetUserDetailsByIdAsync(userId);
            if (user is null) return Unauthorized();
            
            var (accessToken, refreshToken) = await _tokenService.CreateAuthTokens(user);
            SetAuthCookies(accessToken, refreshToken);
            
            return Ok(new
            {
                message = "Tokens created successfully",
                access_token = accessToken,
                refresh_token = refreshToken
            });
        }
        
        private void SetAuthCookies(string accessToken, string refreshToken )
        {
            HttpContext.Response.Cookies.Append(AccessTokenCookie, accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
            
            HttpContext.Response.Cookies.Append(RefreshTokenCookie, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
        }
    }
}
