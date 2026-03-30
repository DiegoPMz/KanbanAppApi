using System.Security.Claims;
using ErrorOr;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Data;
using KanbanAppApi.Domain.BoardAggregate;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Board;

public sealed class DeleteBoard
{
    public record struct DeleteBoardCommand(Guid UserId, Guid Id)
        :ICommand<ErrorOr<string>>;
    
    public class CommandHandler(ApplicationContextDb context) 
        : ICommandHandler<DeleteBoardCommand, ErrorOr<string>> 
    {
        public async ValueTask<ErrorOr<string>> Handle(DeleteBoardCommand command, CancellationToken ct)
        {
            var board = await context.Boards
                .Where(b => b.Id == command.Id &&  b.UserId == command.UserId)
                .FirstOrDefaultAsync(ct);
            
            if (board is null) return 
                BoardErrors.NotFound(command.Id.ToString());
        
            context.Boards.Remove(board);
            await context.SaveChangesAsync(ct);
            
            return "Board deleted successfully";
        }
    }
    
    public static class DeleteBoardEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapDelete("api/boards/{id:guid}", async Task<Results<Ok<string>, ProblemHttpResult>> (
                Guid id, 
                IMediator mediator,
                ClaimsPrincipal user
            ) =>
            {
                if (user.GetUserId() is not { } userId) return 
                    ApiErrorHandler.Problem(Error.Unauthorized());
                
                var result = await mediator.Send(new DeleteBoardCommand(userId,id));

                return !result.IsError
                    ? TypedResults.Ok(result.Value)
                    : ApiErrorHandler.Problem(result.FirstError);
            }).RequireAuthorization();
        }
    }
    
}