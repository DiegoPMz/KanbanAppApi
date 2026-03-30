using System.Security.Claims;
using ErrorOr;
using FluentValidation;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Data;
using KanbanAppApi.Domain.BoardAggregate;
using KanbanAppApi.Features.Board.Shared;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Board;

public sealed class ReorderColumn
{
    public record ReorderColumnRequestDto(List<ColumnOrderInput> Columns);
    
    public record struct ReorderColumnCommand(Guid UserId, Guid BoardId, List<ColumnOrderInput> ReorderedColumns)
        : ICommand<ErrorOr<List<ColumnDto>>>;
    
    public class ReorderColumnHandler(ApplicationContextDb context) 
        : ICommandHandler<ReorderColumnCommand,  ErrorOr<List<ColumnDto>>>
    {
        public async ValueTask<ErrorOr<List<ColumnDto>>> Handle(ReorderColumnCommand command, CancellationToken ct)
        {
            var board = await context.Boards
                .Where(b => b.Id == command.BoardId && b.UserId == command.UserId)
                .Include(b => b.Columns)
                .FirstOrDefaultAsync(ct);
            
            if (board == null) return 
                BoardErrors.NotFound(command.BoardId.ToString());

            var result = board.ReorderColumns(command.ReorderedColumns);
            if (result.IsError) return result.Errors;
           
            await  context.SaveChangesAsync(ct);
            return board.Columns.Select(ColumnDto.FromEntity).ToList();
        }
    }
    
    public class Validator: AbstractValidator<ReorderColumnRequestDto> {
        public Validator()
        {
            RuleFor(x => x.Columns)
                .NotEmpty().WithMessage("At least one column must be provided.");

            RuleForEach(dto => dto.Columns).ChildRules(rc =>
            {
                rc.RuleFor(p => p.NewOrder)
                    .NotNull()
                    .ValidColumnOrder();

                rc.RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Column ID is required.")
                    .NotEqual(Guid.Empty).WithMessage("Invalid Column ID format.");
            });
        }
    }
    
    public static class ReorderColumnEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapPut("api/boards/{boardId:guid}/columns/reorder", async Task<Results<Ok<List<ColumnDto>>, ProblemHttpResult, ValidationProblem>> (
                [FromBody] ReorderColumnRequestDto dto, 
                Guid boardId,
                IMediator mediator,
                ClaimsPrincipal user
            ) =>
            {
                if (user.GetUserId() is not { } userId) return 
                    ApiErrorHandler.Problem(Error.Unauthorized());

                var validation = await new Validator().ValidateAsync(dto);
                
                if (!validation.IsValid) return 
                    TypedResults.ValidationProblem(validation.ToDictionary());
                
                var result = await mediator.Send(new ReorderColumnCommand(userId, boardId, dto.Columns));

                return !result.IsError
                    ? TypedResults.Ok(result.Value)
                    : ApiErrorHandler.Problem(result.FirstError);
            }).RequireAuthorization();
        }
    }
}