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

public sealed class UpdateBoard
{
    public record struct UpdateRequestDto(string Name);
    
    public record struct Command(string Name, Guid Id, Guid UserId);
    
    public interface ICommandHandler
    {
        Task<ErrorOr<BoardDto>> HandleAsync(Command command);
    }

    public class CommandHandler(ApplicationContextDb context) : ICommandHandler
    {
        public async Task<ErrorOr<BoardDto>> HandleAsync(Command command)
        {
            var board = await context.Boards
                .Where(b => b.Id == command.Id &&  b.UserId == command.UserId)
                .FirstOrDefaultAsync();
            
            if (board is null) return BoardErrors.NotFound(command.Id.ToString());
        
            board.Name = board.Name;
            await context.SaveChangesAsync();
        
            return  BoardDto.FromEntity(board);
        }
    }
    
    public class Validator: AbstractValidator<UpdateRequestDto> {
        public Validator()
        {
            RuleFor(u => u.Name).ValidBoardName();
        }
    }
    
    public static class UpdateBoardEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapPut("api/boards/{id:guid}", async Task<Results<Ok<BoardDto>, ProblemHttpResult, ValidationProblem>> (
                [FromBody] UpdateRequestDto dto, 
                Guid id, 
                ICommandHandler handler,
                ClaimsPrincipal user
            ) =>
            {
                if (user.GetUserId() is not { } userId) return ApiErrorHandler.Problem(Error.Unauthorized());
                
                var validation = await new Validator().ValidateAsync(dto);
                if (!validation.IsValid) return TypedResults.ValidationProblem(validation.ToDictionary());
                
                var result = await handler.HandleAsync(new Command(dto.Name, id, userId ));

                return !result.IsError
                    ? TypedResults.Ok(result.Value)
                    : ApiErrorHandler.Problem(result.FirstError);
            }).RequireAuthorization();
        }
    }
    
}