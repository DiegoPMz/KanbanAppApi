using FluentResults;
using FluentValidation;
using KanbanAppApi.Data;
using KanbanAppApi.Features.Board.Common;
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
        Task<Result<BoardDto>> Handle(Command command);
    }

    public class CommandHandler(ApplicationContextDb context) : ICommandHandler
    {
        public async Task<Result<BoardDto>> Handle(Command command)
        {
            var board = await context.Boards
                .Where(b => b.Id == command.Id &&  b.UserId == command.UserId)
                .FirstOrDefaultAsync();
            
            if (board is null) return BoardErrors.NotFound(command.Id.ToString(), nameof(UpdateBoard));
        
            board.Name = board.Name;
            await context.SaveChangesAsync();
        
            return  BoardDto.FromEntity(board);
        }
    }
    
    public class Validator: AbstractValidator<UpdateRequestDto> {
        public Validator()
        {
            RuleFor(u => u.Name)
                .NotEmpty().WithMessage("El nombre no puede estar vacio")
                .MaximumLength(250);
        }
    }
    
    public static class UpdateBoardEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("api/boards/{id:guid}", async Task<Results<Ok<BoardDto>, ProblemHttpResult, ValidationProblem>> (
                [FromBody] UpdateRequestDto dto, 
                Guid id, 
                ICommandHandler handler
            ) =>
            {
                var validation = await new Validator().ValidateAsync(dto);
                if (!validation.IsValid) return TypedResults.ValidationProblem(validation.ToDictionary());
                
                var userId =  Guid.Parse("019ceb7f-e83f-72d2-8ca9-b68d8b2b7f18")!;
                var result = await handler.Handle(new Command(dto.Name, id, userId ));

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value)
                    : TypedResults.Problem();
            });
        }
    }
    
}