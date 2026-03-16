using FluentResults;
using FluentValidation;
using KanbanAppApi.Data;
using KanbanAppApi.Features.Board.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using BoardEntity =  KanbanAppApi.Core.Entities.Board;

namespace KanbanAppApi.Features.Board;

public sealed class CreateBoard
{
        public record struct CreateRequestDto(string Name);
        public record struct Command(Guid UserId, string Name);
        
        public interface ICommandHandler
        {
                Task<Result<BoardDto>> Handle(Command command);
        }
        
        public class CommandHandler(ApplicationContextDb context) : ICommandHandler
        {
                public async Task<Result<BoardDto>> Handle(Command command)
                {
                        var board = new BoardEntity(command.Name,  command.UserId);
                        
                        var boardCreated = await context.Boards.AddAsync(board);
                        await context.SaveChangesAsync();
                        
                        return BoardDto.FromEntity(boardCreated.Entity);
                }
        }
        
        public class Validator: AbstractValidator<CreateRequestDto> {
                public Validator()
                {
                        RuleFor(c => c.Name)
                                .NotEmpty()
                                .MaximumLength(250);
                }
        }

        public static class CreateBoardEndpoint
        {
                public static void Map(WebApplication app)
                {
                        app.MapPost("api/boards",  async Task<Results<Created, ProblemHttpResult, ValidationProblem>> (
                                [FromBody] CreateRequestDto dto, 
                                ICommandHandler handler
                        ) =>
                        {
                                var validation = await new Validator().ValidateAsync(dto);
                                if (!validation.IsValid) return TypedResults.ValidationProblem(validation.ToDictionary());
                                
                                var userId =  Guid.Parse("019ceb7f-e83f-72d2-8ca9-b68d8b2b7f18")!;
                                var result = await handler.Handle(new Command(userId,  dto.Name));

                                return result.IsSuccess 
                                        ? TypedResults.Created() 
                                        : TypedResults.Problem();
                        });
                }
        }
}