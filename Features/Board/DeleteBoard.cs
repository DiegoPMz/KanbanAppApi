using FluentResults;
using KanbanAppApi.Data;
using KanbanAppApi.Features.Board.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Board;

public sealed class DeleteBoard
{
    public record struct Command(Guid UserId, Guid Id);
    
    public interface ICommandHandler
    {
        Task<Result<string>> Handle(Command command);
    }

    public class CommandHandler(ApplicationContextDb context) : ICommandHandler
    {
        public async Task<Result<string>> Handle(Command command)
        {
            var board = await context.Boards
                .Where(b => b.Id == command.Id &&  b.UserId == command.UserId)
                .FirstOrDefaultAsync();
            
            if (board is null) return BoardErrors.NotFound(command.Id.ToString(), nameof(DeleteBoard));
        
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
                ICommandHandler handler
            ) =>
            {
                var userId =  Guid.Parse("019ceb7f-e83f-72d2-8ca9-b68d8b2b7f18")!;
                var result = await handler.Handle(new Command(userId,id));

                return result.IsSuccess 
                    ? TypedResults.Ok(result.Value) 
                    : TypedResults.Problem();
            });
        }
    }
    
}