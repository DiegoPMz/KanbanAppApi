using System.Security.Claims;
using ErrorOr;
using FluentValidation;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Data;
using KanbanAppApi.Domain.TaskAggregate;
using KanbanAppApi.Features.Task.Shared;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Task;

public sealed class AddSubTask
{
    public record struct AddSubTaskRequestDto(string Description, bool? IsCompleted);
    
    public record struct AddSubTaskCommand(Guid UserId, Guid TaskId, string Description, bool? IsCompleted)
        :ICommand<ErrorOr<SubTaskDto>>;
    
    public class AddColumnHandler(ApplicationContextDb context) 
        : ICommandHandler<AddSubTaskCommand,ErrorOr<SubTaskDto>>
    {
        public async ValueTask<ErrorOr<SubTaskDto>> Handle(AddSubTaskCommand command, CancellationToken ct)
        {
            var task = await context.Tasks
                .Where(t => t.Id == command.TaskId)
                .Where(t => context.Boards.Any(b =>
                    b.UserId == command.UserId ))
                .FirstOrDefaultAsync(ct);
            
            if (task is null) 
                return TaskErrors.NotFound(command.TaskId.ToString());
            
            var result = task.AddSubTask(command.Description, command.IsCompleted);

            if (result.IsError) return result.Errors;
            
            await context.SaveChangesAsync(ct);
            return SubTaskDto.FromEntity(result.Value);
        }
    }

    public class Validator : AbstractValidator<AddSubTaskRequestDto>
    {
        public Validator()
        {
            RuleFor(dto => dto.Description)
                .ValidSubTaskDescription();
        }
    }
    
    public static class AddSubTaskEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("api/tasks/{taskId:guid}/subtasks", async (
                AddSubTaskRequestDto dto,
                ClaimsPrincipal user,
                IMediator mediator,
                Guid taskId,
                CancellationToken ct
            ) =>
            {
                if (user.GetUserId() is not { } userId)
                    return ApiErrorHandler.Problem(Error.Unauthorized());
                
                var validation = await new Validator().ValidateAsync(dto, ct);
                if (!validation.IsValid) return TypedResults.ValidationProblem(validation.ToDictionary());
                
                var command = new AddSubTaskCommand(userId,taskId, dto.Description, dto.IsCompleted);
                var result = await mediator.Send(command, ct);

                return result.Match(
                    Results.Ok,
                    errors => ApiErrorHandler.Problem(errors.First())
                );
            });
        }
    }
    
}