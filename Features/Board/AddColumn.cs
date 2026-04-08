using System.Security.Claims;
using ErrorOr;
using FluentValidation;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Common.Domain.BoardAggregate;
using KanbanAppApi.Common.Persistence;
using KanbanAppApi.Features.Board.Shared;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Board;

public sealed class AddColumn
{
    public record struct AddColumnRequestDTo(string Name, string Color);
    
    public record struct AddColumnCommand(Guid UserId, Guid BoardId, string ColumnName, string ColumnColor)
        : ICommand<ErrorOr<ColumnDto>>;
    
    public class AddColumnHandler(ApplicationContextDb context)
        :ICommandHandler<AddColumnCommand,  ErrorOr<ColumnDto>>
    {
        public async ValueTask<ErrorOr<ColumnDto>> Handle(AddColumnCommand command, CancellationToken ct)
        {
            var board = await context.Boards
                .Include(b => b.Columns) 
                .Where(b => b.UserId == command.UserId && b.Id == command.BoardId)
                .FirstOrDefaultAsync(ct);
            
            if (board is null) return 
                BoardErrors.NotFound(command.BoardId.ToString());
            
            var result = board.AddColumn(command.ColumnName, command.ColumnColor);
            if (result.IsError) return result.Errors;
            
            await context.SaveChangesAsync(ct);
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
                ClaimsPrincipal user,
                IMediator mediator
            ) =>
            {
                if (user.GetUserId() is not { } userId) 
                    return ApiErrorHandler.Problem(Error.Unauthorized());
                
                var validation = await new Validator().ValidateAsync(dto);
                
                if (!validation.IsValid) return 
                    TypedResults.ValidationProblem(validation.ToDictionary());

                var result = await mediator.Send(new AddColumnCommand(userId, boardId, dto.Name, dto.Color));
                
                return !result.IsError
                    ? TypedResults.Ok(result.Value)
                    : ApiErrorHandler.Problem(result.FirstError);
            }).RequireAuthorization();
        }
    }
}