using System.Security.Claims;
using ErrorOr;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Data;
using KanbanAppApi.Domain.BoardAggregate;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Board;

public class RemoveColumn
{
    public record struct Command(Guid UserId,Guid BoardId, Guid ColumnId);
    
    public interface ICommandHandler
    {
        Task<ErrorOr<string>> HandleAsync(Command command);
    }

    public class CommandHandler(ApplicationContextDb context) : ICommandHandler
    {
        public async Task<ErrorOr<string>> HandleAsync(Command command)
        {
            var board = await context.Boards
                .Include(b => b.Columns) 
                .Where(b => b.UserId == command.UserId && b.Id == command.BoardId)
                .FirstOrDefaultAsync();
            
            if (board is null) return BoardErrors.NotFound(command.BoardId.ToString());
            
            var result = board.RemoveColumn(command.ColumnId);
            if (result.IsError) return result.Errors;
            
            await context.SaveChangesAsync();
            return result.Value;
        }
    }
    
    public static class RemoveColumnEndpoint
    {
        
        public static void Map(WebApplication app)
        {
            app.MapDelete("api/boards/{boardId:guid}/columns/{columnId:guid}",  async Task<Results<Ok<string>, ProblemHttpResult, ValidationProblem>> (
                Guid boardId,
                Guid columnId,
                ICommandHandler handler,
                ClaimsPrincipal user
            ) =>
            {
                if (user.GetUserId() is not { } userId) return ApiErrorHandler.Problem(Error.Unauthorized());
                var result = await handler.HandleAsync(new Command(userId, boardId, columnId));
                
                return !result.IsError
                    ? TypedResults.Ok(result.Value)
                    : ApiErrorHandler.Problem(result.FirstError);
            }).RequireAuthorization();
        }
    }
    
}