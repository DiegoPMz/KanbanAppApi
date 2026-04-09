using System.Security.Claims;
using ErrorOr;
using KanbanAppApi.Common.Domain.User;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Common.Persistence;
using KanbanAppApi.Features.User.Shared;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.User;

public sealed class GetUser
{
    public record struct GetUserCommand(Guid Id)
        : ICommand<ErrorOr<UserDto>>;
    
    public class GetUserHandler(ApplicationContextDb context)
        : ICommandHandler<GetUserCommand, ErrorOr<UserDto>>
    {
        public async ValueTask<ErrorOr<UserDto>> Handle(GetUserCommand command, CancellationToken ct)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == command.Id, ct);

            if (user is null) return UserErrors.NotFound;
            
            return new UserDto(user.Email, user.Id, user.AppTheme);
        }
    }
    
    public static class GetUserEndPoint
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("api/users/me", async Task<Results<Ok<UserDto>, ProblemHttpResult>> (
                CancellationToken ct,
                IMediator mediator,
                ClaimsPrincipal user
            ) =>
            {
                if (user.GetUserId() is not { } userId)
                    return ApiErrorHandler.Problem(Error.Unauthorized());
                
                var result = await mediator.Send(new GetUserCommand(userId), ct); 
                
                if (result.IsError)
                    return ApiErrorHandler.Problem(result.FirstError);
                
                return TypedResults.Ok(result.Value);
            }).RequireAuthorization();
        }
    }
    
}