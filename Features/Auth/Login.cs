using System.Security.Claims;
using ErrorOr;
using KanbanAppApi.Data;
using KanbanAppApi.Features.Auth.Infrastructure;
using KanbanAppApi.Features.Auth.Shared;
using Mediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserModel = KanbanAppApi.Domain.User.User;

namespace KanbanAppApi.Features.Auth;

public sealed class Login
{
    public record struct ExternalUserIdentity(
        string ProviderKey,
        string Email,
        bool IsEmailVerified,
        string? UrlPicture,
        string? Name,
        string? GivenName,
        string? FamilyName,
        string Provider
    );
    
    public record ExternalCodeExchangeResponse(
         string AccessToken,
         string? RefreshToken,
         string? Scope,
         string TokenType,
         string TokenId
     );
    
    public interface IExternalAuthProvider
    {
        public ErrorOr<string> GetProviderPkceUrl(string codeChallenge);
        public Task<ErrorOr<ExternalCodeExchangeResponse>> ExchangeCodeForTokenAsync(string code, string codeVerifier, CancellationToken ct = default);
        public Task<ErrorOr<ExternalUserIdentity>> GetUserDetailsAsync(string tokenId, CancellationToken ct = default);
    }
    
    public record struct PkceLoginResponseDto(string CodeVerifier, string Url);
    public record struct LoginCommand : ICommand<ErrorOr<PkceLoginResponseDto>>;
    
    public class LoginCommandHandler(IExternalAuthProvider externalAuthProvider)
        :ICommandHandler<LoginCommand, ErrorOr<PkceLoginResponseDto>>
    {
        public ValueTask<ErrorOr<PkceLoginResponseDto>> Handle(LoginCommand command, CancellationToken ct)
        {
            var pkceCodes = PkceUtilities.GenerateCodes();
            
            var providerUrl = externalAuthProvider.GetProviderPkceUrl(pkceCodes.CodeChallenge);

            return providerUrl.IsError 
                ? ValueTask.FromResult<ErrorOr<PkceLoginResponseDto>>(Error.Unexpected("We have an internal error. Please try again later.")) 
                : ValueTask.FromResult<ErrorOr<PkceLoginResponseDto>>(new PkceLoginResponseDto(pkceCodes.CodeVerifier, providerUrl.Value));
        }
    }
    
    public record struct LoginCallbackResponseDto(TokenDto Token, string AuthenticationScheme, ClaimsPrincipal ClaimsPrincipal);
    public record struct LoginCallBackCommand(string Code, string CodeVerifier)
        : ICommand<ErrorOr<LoginCallbackResponseDto>>;
    
    public class LoginCallBackCommandHandler(ApplicationContextDb context, IExternalAuthProvider externalAuthProvider)
        :ICommandHandler<LoginCallBackCommand, ErrorOr<LoginCallbackResponseDto>>
    {
        public async ValueTask<ErrorOr<LoginCallbackResponseDto>> Handle(LoginCallBackCommand command, CancellationToken ct)
        {
           var exchangeResult = await externalAuthProvider.ExchangeCodeForTokenAsync(command.Code, command.CodeVerifier, ct);

           if (exchangeResult.IsError) return exchangeResult.Errors;
           
           var userDetails = await externalAuthProvider.GetUserDetailsAsync(exchangeResult.Value.TokenId, ct);
           
           if (userDetails.IsError) return userDetails.Errors;
           
           var user = await context.Users
               .FirstOrDefaultAsync(u => u.ExternalId == userDetails.Value.ProviderKey, ct);
           
           if (user is null)
           {
               var newUser = UserModel.Create(
                   userDetails.Value.ProviderKey,
                   userDetails.Value.Email,
                   userDetails.Value.Name,
                   userDetails.Value.FamilyName,
                   userDetails.Value.UrlPicture
               );
        
               if (newUser.IsError) return newUser.Errors;
        
               user = newUser.Value;
               await context.Users.AddAsync(user, ct);
           }
           
           var accessToken = Token.Create(
               user.Id,
               userDetails.Value.Provider,
               TokenType.AccessToken,
               DateTime.UtcNow + TimeSpan.FromMinutes(15)
           );

           if (accessToken.IsError) 
               return accessToken.Errors;
           
           await context.Tokens.AddAsync(accessToken.Value, ct);
           await context.SaveChangesAsync(ct);
           
           var principal = new ClaimsPrincipal(
               new ClaimsIdentity([
                   new Claim("userId", accessToken.Value.UserId.ToString()),
                   new Claim("sessionId", accessToken.Value.SessionId.ToString()), 
                   new Claim(ClaimTypes.Role, "User"),
               ],
               CookieAuthenticationDefaults.AuthenticationScheme
           ));
           
           return new LoginCallbackResponseDto(
               new TokenDto(accessToken.Value.SessionId, accessToken.Value.ExpiresAt),
               CookieAuthenticationDefaults.AuthenticationScheme,
               principal
           );
        }
    }
    
    public static class LoginEndPoint
    {
        private const string CodeVerifierCookieName = "code_verifier";
        
        public static void Map(WebApplication app)
        {
            app.MapGet("api/auth/login", async Task<RedirectHttpResult> (
                CancellationToken ct,
                IMediator mediator,
                IConfiguration  configuration,
                HttpContext context
            ) =>
            {
                var clientBaseUri = configuration["Client:AuthRedirectUri"] ?? "http://localhost:5173";
                var loginPage = $"{clientBaseUri}/login";
                
                var result = await mediator.Send(new LoginCommand(), ct);

                if (result.IsError)
                    return TypedResults.Redirect($"{loginPage}?error={result.FirstError.Code}");
                
                context.Response.Cookies.Append(
                    CodeVerifierCookieName, 
                    result.Value.CodeVerifier, 
                    new CookieOptions
                    {
                        SameSite = SameSiteMode.Lax,
                        Secure = true,
                        HttpOnly = true,
                        Path = "/api/auth",
                        Expires = DateTimeOffset.UtcNow.AddMinutes(14) 
                    }
                );

                return TypedResults.Redirect(result.Value.Url);
            });
            
            app.MapGet("api/auth/login-callback", async  Task<RedirectHttpResult> (
                CancellationToken ct,
                [FromQuery] string code,
                HttpContext context,
                IConfiguration configuration,
                IMediator mediator
            ) =>
            {
                context.Request
                    .Cookies
                    .TryGetValue(CodeVerifierCookieName, out var codeVerifier);

                var clientBaseUri = configuration["Client:AuthRedirectUri"] ?? "http://localhost:5173";
                var loginPage = $"{clientBaseUri}/login";
                
                if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(codeVerifier)) 
                    return TypedResults.Redirect(loginPage);
                
                var command = new LoginCallBackCommand(code, codeVerifier);
                var result = await mediator.Send(command, ct);
                
                context.Response.Cookies.Delete(CodeVerifierCookieName);
                
                if (result.IsError) 
                {
                    var error = result.FirstError;
                    return TypedResults.Redirect($"{loginPage}?error={error.Code}");
                }
                
                await context.SignInAsync(
                    result.Value.AuthenticationScheme, 
                    result.Value.ClaimsPrincipal, 
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = result.Value.Token.ExpiresAt,
                    }
                );
                
                return TypedResults.Redirect(clientBaseUri);
            });
        }
    }
}
