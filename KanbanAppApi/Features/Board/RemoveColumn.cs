using System.Security.Claims;
using ErrorOr;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Common.Domain.BoardAggregate;
using KanbanAppApi.Common.Persistence;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Board;

public sealed class RemoveColumn
{
    public record struct RemoveColumnCommand(Guid UserId,Guid BoardId, Guid ColumnId)
        :ICommand<ErrorOr<string>>;
    
    public class RemoveColumnHandler(ApplicationContextDb context)
        :ICommandHandler<RemoveColumnCommand, ErrorOr<string>>
    {
        public async ValueTask<ErrorOr<string>> Handle(RemoveColumnCommand command, CancellationToken ct)
        {
            var board = await context.Boards
                .Include(b => b.Columns) 
                .Where(b => b.UserId == command.UserId && b.Id == command.BoardId)
                .FirstOrDefaultAsync(ct);
            
            if (board is null) 
                return BoardErrors.NotFound(command.BoardId.ToString());
            
            var result = board.RemoveColumn(command.ColumnId);
            if (result.IsError) return result.Errors;
            
            await context.SaveChangesAsync(ct);
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
                IMediator mediator,
                ClaimsPrincipal user
            ) =>
            {
                if (user.GetUserId() is not { } userId) 
                    return ApiErrorHandler.Problem(Error.Unauthorized());
                
                var result = await mediator.Send(new RemoveColumnCommand(userId, boardId, columnId));
                
                return !result.IsError
                    ? TypedResults.Ok(result.Value)
                    : ApiErrorHandler.Problem(result.FirstError);
            }).RequireAuthorization();
        }
    }
    
}