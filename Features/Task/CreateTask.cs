using System.Security.Claims;
using ErrorOr;
using KanbanAppApi.Common.Domain.TaskAggregate;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Common.Persistence;
using KanbanAppApi.Features.Task.Shared;
using Mediator;
using Microsoft.EntityFrameworkCore;
using TaskModel = KanbanAppApi.Common.Domain.TaskAggregate.Task;

namespace KanbanAppApi.Features.Task;

public class CreateTask
{
    public record struct CreateTaskRequestDto(
        Guid ColumnId,
        string Title, 
        PriorityType Priority,
        string? Description,
        bool? IsCompleted
    );
    
    public record struct CreateTaskCommand(
        Guid UserId,
        Guid ColumnId,
        string Title, 
        PriorityType Priority,
        string? Description,
        bool? IsCompleted
    ): ICommand<ErrorOr<TaskDto>>;
    
    
    public sealed class CreateTaskHandler(ApplicationContextDb context) 
        : ICommandHandler<CreateTaskCommand, ErrorOr<TaskDto>>
    {
        public async ValueTask<ErrorOr<TaskDto>> Handle(CreateTaskCommand command, CancellationToken ct)
        {
            var isDataValid = await context.Boards
                .AsNoTracking()
                .Where(b => b.UserId == command.UserId)
                .AnyAsync(b => b.Columns.Any(c => c.Id == command.ColumnId), ct);
            
            if (!isDataValid) return Error.Validation("The column or board sent doesn't exist.");
    
            var task = new TaskModel(
                command.Title,
                command.Priority, 
                command.ColumnId,
                command.Description,
                command.IsCompleted
            );
    
            var taskCreated = context.Tasks.Add(task);
            await context.SaveChangesAsync(ct);
    
            return TaskDto.FromEntity(taskCreated.Entity);
        }
    }
    
    public static class CreateTaskEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("api/tasks", async (
                CreateTaskRequestDto dto,
                ClaimsPrincipal user,
                IMediator mediator,
                CancellationToken ct
            ) =>
            {
                if (user.GetUserId() is not { } userId)
                    return ApiErrorHandler.Problem(Error.Unauthorized());
                
                var command = new CreateTaskCommand(
                    userId,
                    dto.ColumnId,
                    dto.Title,
                    dto.Priority,
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