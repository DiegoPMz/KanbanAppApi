using System.Security.Claims;
using ErrorOr;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Common.Domain.TaskAggregate;
using KanbanAppApi.Common.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Task;

public sealed class RemoveSubTask
{
    public record struct RemoveSubTaskCommand(Guid UserId,Guid TaskId, Guid SubTaskId)
        :ICommand<ErrorOr<string>>;
    
    public class RemoveSubTaskHandler(ApplicationContextDb context)
        :ICommandHandler<RemoveSubTaskCommand, ErrorOr<string>>
    {
        public async ValueTask<ErrorOr<string>> Handle(RemoveSubTaskCommand command, CancellationToken ct)
        {
            var task = await context.Tasks
                .Where(t => t.Id == command.TaskId)
                .Where(t => context.Boards.Any(b =>
                    b.UserId == command.UserId ))
                .FirstOrDefaultAsync(ct);
            
            if (task is null) return 
                TaskErrors.NotFound(command.TaskId.ToString());

            var result = task.RemoveSubTask(command.SubTaskId);
            if (!result.IsError) return result.Errors;
            
            await context.SaveChangesAsync(ct);
            return result.Value;
        }
    }
    
    public static class RemoveSubTaskEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapDelete("api/tasks/{taskId:guid}/subtasks/{subTaskId:guid}", async (
                ClaimsPrincipal user,
                IMediator mediator,
                Guid taskId,
                Guid subTaskId,
                CancellationToken ct
            ) =>
            {
                if (user.GetUserId() is not { } userId)
                    return ApiErrorHandler.Problem(Error.Unauthorized());
                
                var result = await mediator.Send(new RemoveSubTaskCommand(userId,  taskId, subTaskId), ct);

                return result.Match(
                    Results.Ok,
                    errors => ApiErrorHandler.Problem(errors.First())
                );
            }).RequireAuthorization();
        }
    }
    
    
}