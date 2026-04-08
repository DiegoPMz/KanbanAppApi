using ErrorOr;
using KanbanAppApi.Common.Persistence;
using Mediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Auth;

public sealed class Logout
{
    public record struct LogoutCommand(Guid SessionId)
        :ICommand<ErrorOr<Success>>;
    
    public class LogoutCommandHandler(ApplicationContextDb context) 
        : ICommandHandler<LogoutCommand, ErrorOr<Success>>
    {
        public async ValueTask<ErrorOr<Success>> Handle(LogoutCommand command, CancellationToken ct)
        {
            var token = await context.Tokens
                .FirstOrDefaultAsync(t => t.SessionId == command.SessionId, ct);
    
            if (token is not null)
            {
                context.Tokens.Remove(token);
                await context.SaveChangesAsync(ct);
            }
            
            return Result.Success;
        }
    }

    public static class LogoutEndPoint
    {
        private const string SessionKey = "session";
        
        public static void Map(WebApplication app)
        {
            app.MapPost("api/auth/logout", async Task<Ok> (
                IMediator mediator,
                HttpContext context,
                CancellationToken ct
            ) =>
            {
                var rawSessionId = context.Request.Cookies[SessionKey];

                if (!string.IsNullOrEmpty(rawSessionId) && Guid.TryParse(rawSessionId, out var sessionId))
                {
                    await mediator.Send(new LogoutCommand(sessionId), ct);
                }
                
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                
                return TypedResults.Ok();
            }).RequireAuthorization();
        }
    }
    
}