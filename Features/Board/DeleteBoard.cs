using System.Security.Claims;
using ErrorOr;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Data;
using KanbanAppApi.Domain.BoardAggregate;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Board;

public sealed class DeleteBoard
{
    public record struct Command(Guid UserId, Guid Id);
    
    public interface ICommandHandler
    {
        Task<ErrorOr<string>> HandleAsync(Command command);
    }

    public class CommandHandler(ApplicationContextDb context) : ICommandHandler
    {
        public async Task<ErrorOr<string>> HandleAsync(Command command)
        {
            var board = await context.Boards
                .Where(b => b.Id == command.Id &&  b.UserId == command.UserId)
                .FirstOrDefaultAsync();
            
            if (board is null) return BoardErrors.NotFound(command.Id.ToString());
        
            context.Boards.Remove(board);
            await context.SaveChangesAsync();
            
            return "Board deleted successfully";
        }
    }
    
    public static class DeleteBoardEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapDelete("api/boards/{id:guid}", async Task<Results<Ok<string>, ProblemHttpResult>> (
                Guid id, 
                ICommandHandler handler,
                ClaimsPrincipal user
            ) =>
            {
                if (user.GetUserId() is not { } userId) return ApiErrorHandler.Problem(Error.Unauthorized());
                var result = await handler.HandleAsync(new Command(userId,id));

                return !result.IsError
                    ? TypedResults.Ok(result.Value)
                    : ApiErrorHandler.Problem(result.FirstError);
            }).RequireAuthorization();
        }
    }
    
}