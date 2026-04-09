using System.Security.Claims;
using ErrorOr;
using FluentValidation;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Common.Domain.TaskAggregate;
using KanbanAppApi.Common.Persistence;
using KanbanAppApi.Features.Task.Shared;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Task;

public sealed class UpdateSubTask
{
    public record struct UpdateSubTaskRequestDto(
        string? Description,
        bool? IsCompleted
    );
    
    public record struct UpdateSubTaskCommand(
        Guid UserId,
        Guid TaskId,
        Guid SubTaskId,
        string? Description,
        bool? IsCompleted
    ) : ICommand<ErrorOr<SubTaskDto>>;
    
    public class UpdateSubTaskHandler(ApplicationContextDb context)
        : ICommandHandler<UpdateSubTaskCommand, ErrorOr<SubTaskDto>>
    {
        public async ValueTask<ErrorOr<SubTaskDto>> Handle(UpdateSubTaskCommand command, CancellationToken ct)
        {
            var task = await context.Tasks
                .Where(t => t.Id == command.TaskId)
                .Where(t => context.Boards.Any(b =>
                    b.UserId == command.UserId ))
                .FirstOrDefaultAsync(ct);
            
            if (task is null) return 
                TaskErrors.NotFound(command.TaskId.ToString());

            var result =  task.EditSubTask(
                command.SubTaskId, 
                command.Description, 
                command.IsCompleted
            );

            if (result.IsError) return  result.Errors;
            
            await context.SaveChangesAsync(ct);
            return SubTaskDto.FromEntity(result.Value);
        }
    }
    
     public class Validator: AbstractValidator<UpdateSubTaskRequestDto> {
        public Validator()
        {
            RuleFor(x => x.Description)
                .ValidSubTaskDescription()
                .When(x => x.Description is not null);
            
            RuleFor(x => x.IsCompleted)
                .ValidTaskStatus()
                .When(x => x.IsCompleted is not null);
            
            RuleFor(x => x)
                .Must(HaveAtLeastOneProperty)
                .WithMessage("At least one property is required to perform an update.");
        }
        
        private bool HaveAtLeastOneProperty(UpdateSubTaskRequestDto dto)
        {
            return new object?[] { dto.Description, dto.IsCompleted }
                .Any(p => p is not null);
        }
    }

    public static class UpdateSubTaskEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapPut("api/tasks/{taskId:guid}/subtasks/{subTaskId:guid}", async (
                UpdateSubTaskRequestDto dto,
                Guid taskId,
                Guid subTaskId,
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
                
                var command = new UpdateSubTaskCommand(
                    userId,
                    taskId,
                    subTaskId,
                    dto.Description,
                    dto.IsCompleted
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