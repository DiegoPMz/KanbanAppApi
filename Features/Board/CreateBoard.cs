using System.Security.Claims;
using ErrorOr;
using FluentValidation;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Common.Persistence;
using KanbanAppApi.Features.Board.Shared;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using BoardEntity = KanbanAppApi.Common.Domain.BoardAggregate.Board;
using Error = ErrorOr.Error;

namespace KanbanAppApi.Features.Board;

public sealed class CreateBoard
{
    public record struct CreateRequestDto(string Name);

    public record struct CreateBoardCommand(Guid UserId, string Name) 
        :ICommand<ErrorOr<BoardDto>>;
    
    public class CreateBoardHandler(ApplicationContextDb context)
        :ICommandHandler<CreateBoardCommand,ErrorOr<BoardDto>>
    {
        public async ValueTask<ErrorOr<BoardDto>> Handle(CreateBoardCommand command, CancellationToken ct)
        {
            var board = new BoardEntity(command.Name, command.UserId);

            var boardCreated = await context.Boards.AddAsync(board,ct);
            await context.SaveChangesAsync(ct);

            return BoardDto.FromEntity(boardCreated.Entity);
        }
    }

    public class Validator : AbstractValidator<CreateRequestDto>
    {
        public Validator()
        {
            RuleFor(c => c.Name)
                .ValidBoardName();
        }
    }

    public static class CreateBoardEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("api/boards", async Task<Results<Created<BoardDto>, ProblemHttpResult, ValidationProblem>> (
                [FromBody] CreateRequestDto dto,
                ClaimsPrincipal user,
                IMediator mediator
            ) =>
            {
                if (user.GetUserId() is not { } userId) return 
                    ApiErrorHandler.Problem(Error.Unauthorized());

                var validation = await new Validator().ValidateAsync(dto);
                
                if (!validation.IsValid) 
                    return TypedResults.ValidationProblem(validation.ToDictionary());

                var result = await mediator.Send(new CreateBoardCommand(userId, dto.Name));
                
                return !result.IsError
                    ? TypedResults.Created(string.Empty,result.Value)
                    : ApiErrorHandler.Problem(result.FirstError);
            }).RequireAuthorization();
        }
    }
}