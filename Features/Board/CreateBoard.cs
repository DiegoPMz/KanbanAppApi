using System.Security.Claims;
using FluentResults;
using FluentValidation;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Data;
using KanbanAppApi.Features.Board.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using BoardEntity = KanbanAppApi.Domain.BoardAggregate.Board;
using Error = ErrorOr.Error;

namespace KanbanAppApi.Features.Board;

public sealed class CreateBoard
{
    public record struct CreateRequestDto(string Name);

    public record struct Command(Guid UserId, string Name);

    public interface ICommandHandler
    {
        Task<Result<BoardDto>> HandleAsync(Command command);
    }

    public class CommandHandler(ApplicationContextDb context) : ICommandHandler
    {
        public async Task<Result<BoardDto>> HandleAsync(Command command)
        {
            var board = new BoardEntity(command.Name, command.UserId);

            var boardCreated = await context.Boards.AddAsync(board);
            await context.SaveChangesAsync();

            return BoardDto.FromEntity(boardCreated.Entity);
        }
    }

    public class Validator : AbstractValidator<CreateRequestDto>
    {
        public Validator()
        {
            RuleFor(c => c.Name).ValidBoardName();
        }
    }

    public static class CreateBoardEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("api/boards", async Task<Results<Created, ProblemHttpResult, ValidationProblem>> (
                [FromBody] CreateRequestDto dto,
                ICommandHandler handler,
                ClaimsPrincipal user
            ) =>
            {
                if (user.GetUserId() is not { } userId) return ApiErrorHandler.Problem(Error.Unauthorized());

                var validation = await new Validator().ValidateAsync(dto);
                if (!validation.IsValid) return TypedResults.ValidationProblem(validation.ToDictionary());

                var result = await handler.HandleAsync(new Command(userId, dto.Name));

                return result.IsSuccess
                    ? TypedResults.Created()
                    : TypedResults.Problem();
            }).RequireAuthorization();
        }
    }
}