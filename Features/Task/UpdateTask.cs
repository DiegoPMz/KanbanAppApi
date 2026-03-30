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

public class UpdateTask
{
    public record struct UpdateTaskRequestDto(
        string? Title,
        string? Description,
        bool? IsCompleted,
        PriorityType? Priority
    );

    public record struct UpdateTaskCommand(
        Guid UserId,
        Guid TaskId,
        string? Title,
        string? Description,
        bool? IsCompleted,
        PriorityType? Priority
    ) : ICommand<ErrorOr<TaskDto>>;

    public class UpdateTaskHandler(ApplicationContextDb context) 
        : ICommandHandler<UpdateTaskCommand, ErrorOr<TaskDto>>
    {
        public async ValueTask<ErrorOr<TaskDto>> Handle(UpdateTaskCommand command, CancellationToken ct)
        {
            var task = await context.Tasks
                .Where(t => t.Id == command.TaskId)
                .Where(t => context.Boards.Any(b =>
                    b.UserId == command.UserId &&
                    b.Columns.Any(c => c.Id == t.ColumnId)))
                .FirstOrDefaultAsync(ct);
            
            if (task is null) return 
                TaskErrors.NotFound(command.TaskId.ToString());

            var result = task.Update(
                command.Title, 
                command.Description, 
                command.IsCompleted, 
                command.Priority
            );

            if (result.IsError) return result.Errors;

            await context.SaveChangesAsync(ct);
            return TaskDto.FromEntity(task);
        }
    }
    
    public class Validator: AbstractValidator<UpdateTaskRequestDto> {
        public Validator()
        {
            RuleFor(x => x.Title)
                .ValidTaskTitle()
                .When(x => x.Title is not null);
            
            RuleFor(x => x.Description)
                .ValidTaskDescription()
                .When(x => x.Description is not null);
            
            RuleFor(x => x.Priority)
                .ValidTaskPriority()
                .When(x => x.Priority is not null);

            RuleFor(x => x.IsCompleted)
                .ValidTaskStatus()
                .When(x => x.IsCompleted is not null);
            
            RuleFor(x => x)
                .Must(HaveAtLeastOneProperty)
                .WithMessage("At least one property is required to perform an update.");
        }
        
        private bool HaveAtLeastOneProperty(UpdateTaskRequestDto model)
        {
            return new object?[] { model.Title, model.Description, model.Priority, model.IsCompleted }
                .Any(p => p is not null);
        }
    }

    public static class UpdateTaskEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapPut("api/tasks/{taskId:guid}", async (
                UpdateTaskRequestDto dto,
                Guid taskId,
                ClaimsPrincipal user,
                IMediator mediator,
                CancellationToken ct
            ) =>
            {
                if (user.GetUserId() is not { } userId)
                    return ApiErrorHandler.Problem(Error.Unauthorized());
                
                var validation = await new Validator().ValidateAsync(dto, ct);
                if (!validation.IsValid) 
                    return TypedResults.ValidationProblem(validation.ToDictionary());
                
                var command = new UpdateTaskCommand(
                    userId,
                    taskId,
                    dto.Title,
                    dto.Description,
                    dto.IsCompleted,
                    dto.Priority
                );

                var result = await mediator.Send(command, ct);

                return result.Match(
                    Results.Ok,
                    errors => ApiErrorHandler.Problem(errors.First())
                );
            }).RequireAuthorization();
        }
    }
}