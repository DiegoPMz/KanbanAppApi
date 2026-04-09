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

public class UpdateColumn
{
    public record struct UpdateColumnRequestDTo(string? Name, string? Color);
    public record struct UpdateColumnCommand(Guid UserId, Guid BoardId, Guid ColumnId, string? Name, string? Color)
        :ICommand<ErrorOr<ColumnDto>>;
    
    public class UpdateColumnHandler(ApplicationContextDb context) 
        : ICommandHandler<UpdateColumnCommand,ErrorOr<ColumnDto>>
    {
        public async ValueTask<ErrorOr<ColumnDto>> Handle(UpdateColumnCommand command, CancellationToken ct)
        {
            var board = await context.Boards
                .Include(b => b.Columns) 
                .Where(b => b.UserId == command.UserId && b.Id == command.BoardId)
                .FirstOrDefaultAsync(ct);
            
            if (board is null) return 
                BoardErrors.NotFound(command.BoardId.ToString());
            
            var result = board.EditColumn(command.ColumnId, command.Name, command.Color);
            if (result.IsError) return result.Errors;
            
            await context.SaveChangesAsync(ct);
            return ColumnDto.FromEntity(result.Value);
        }
    }
    
    public class Validator: AbstractValidator<UpdateColumnRequestDTo> {
        public Validator()
        {
            RuleFor(x => x.Name)
                .ValidColumnName()
                .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Color)
                .ValidColumnColor()
                .When(x => !string.IsNullOrWhiteSpace(x.Color));
            
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Name) || !string.IsNullOrWhiteSpace(x.Color))
                .WithMessage("You should send at least one property in order to update the column.");
        }
    }
    
    public static class UpdateColumnEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapPut("api/boards/{boardId:guid}/columns/{columnId:guid}", async Task<Results<Ok<ColumnDto>, ProblemHttpResult, ValidationProblem>> (
                [FromBody] UpdateColumnRequestDTo dto, 
                Guid boardId,
                Guid columnId,
                IMediator mediator,
                ClaimsPrincipal user
            ) =>
            {
                if (user.GetUserId() is not { } userId) 
                    return ApiErrorHandler.Problem(Error.Unauthorized());
                
                var validation = await new Validator().ValidateAsync(dto);
                
                if (!validation.IsValid) 
                    return TypedResults.ValidationProblem(validation.ToDictionary());
                
                var result = await mediator.Send(new UpdateColumnCommand(userId, boardId, columnId, dto.Name, dto.Color));

                return !result.IsError
                    ? TypedResults.Ok(result.Value)
                    : ApiErrorHandler.Problem(result.FirstError);
            }).RequireAuthorization();
        }
    }
    
}