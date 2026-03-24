using System.Security.Claims;
using ErrorOr;
using FluentValidation;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Data;
using KanbanAppApi.Domain.BoardAggregate;
using KanbanAppApi.Features.Board.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Board;

public class AddColumn
{
    public record struct Command(Guid UserId, Guid BoardId, string ColumnName, string ColumnColor);
    
    public record struct AddColumnRequestDTo(string Name, string Color);
    
    public interface ICommandHandler
    {
        Task<ErrorOr<ColumnDto>> HandleAsync(Command command);
    }

    public class CommandHandler(ApplicationContextDb context) : ICommandHandler
    {
        public async Task<ErrorOr<ColumnDto>> HandleAsync(Command command)
        {
            var board = await context.Boards
                .Include(b => b.Columns) 
                .Where(b => b.UserId == command.UserId && b.Id == command.BoardId)
                .FirstOrDefaultAsync();
            
            if (board is null) return BoardErrors.NotFound(command.BoardId.ToString());
            
            var result = board.AddColumn(command.ColumnName, command.ColumnColor);
            if (result.IsError) return result.Errors;
            
            await context.SaveChangesAsync();
            return ColumnDto.FromEntity(result.Value);
        }
    }
    
    public class Validator : AbstractValidator<AddColumnRequestDTo>
    {
        public Validator()
        {
            RuleFor(a => a.Name).ValidColumnName();
            RuleFor(a => a.Color).ValidColumnColor();
        }
    }

    public static class AddColumnEndpoint
    {
        
        public static void Map(WebApplication app)
        {
            app.MapPost("api/boards/{boardId:guid}/columns",  async Task<Results<Ok<ColumnDto>, ProblemHttpResult, ValidationProblem>> (
                Guid boardId,
                [FromBody] AddColumnRequestDTo dto,
                ICommandHandler handler,
                ClaimsPrincipal user
            ) =>
            {
                if (user.GetUserId() is not { } userId) return ApiErrorHandler.Problem(Error.Unauthorized());
                
                var validation = await new Validator().ValidateAsync(dto);
                if (!validation.IsValid) return TypedResults.ValidationProblem(validation.ToDictionary());

                var result = await handler.HandleAsync(new Command(userId, boardId, dto.Name, dto.Color));
                
                return !result.IsError
                    ? TypedResults.Ok(result.Value)
                    : ApiErrorHandler.Problem(result.FirstError);
            }).RequireAuthorization();
        }
    }
}