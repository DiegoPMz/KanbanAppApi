using System.Security.Claims;
using ErrorOr;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Common.Domain.TaskAggregate;
using KanbanAppApi.Common.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Task;

public class DeleteTask
{
    public record struct DeleteTaskCommand(Guid UserId, Guid TaskId)
        : ICommand<ErrorOr<string>>;
    
    public class DeleteTaskHandler(ApplicationContextDb context) 
        : ICommandHandler<DeleteTaskCommand, ErrorOr<string>>
    {
        public async ValueTask<ErrorOr<string>> Handle(DeleteTaskCommand command, CancellationToken ct)
        {
            var task = await context.Tasks
                .Where(t => t.Id == command.TaskId)
                .Where(t => context.Boards.Any(b =>
                    b.UserId == command.UserId &&
                    b.Columns.Any(c => c.Id == t.ColumnId)))
                .FirstOrDefaultAsync(ct);
            
            if (task is null) return TaskErrors.NotFound(command.TaskId.ToString());
            
            task.Delete();
            context.Remove(task);
            await context.SaveChangesAsync(ct);
            
            return "Task successfully deleted";
        }
    }
    
    public static class DeleteTaskEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapDelete("api/tasks/{taskId:guid}", async (
                ClaimsPrincipal user,
                Guid taskId,
                IMediator mediator,
                CancellationToken ct
            ) =>
            {
                if (user.GetUserId() is not { } userId)
                    return ApiErrorHandler.Problem(Error.Unauthorized());
                
                var result = await mediator.Send(new DeleteTaskCommand(userId, taskId), ct);

                return result.Match(
                    Results.Ok,
                    errors => ApiErrorHandler.Problem(errors.First())
                );
            }).RequireAuthorization();
        }
    }
}