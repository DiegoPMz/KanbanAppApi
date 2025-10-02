using KanbanAppApi.Dtos;
using KanbanAppApi.Filters;
using KanbanAppApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KanbanAppApi.Controllers
{
    [Authorize]
    [RequireUserId]
    [ApiController]
    [Route("api/subTasks")]
    public class SubTaskController : ControllerBase
    {
        private readonly ISubTaskService _subTaskService;

        public SubTaskController(ISubTaskService subTaskService)
        {
            _subTaskService = subTaskService;
        }

        [HttpPost]
        public async Task<IResult> CreateSubTask([FromBody] CreateSubTaskRequestDto subTaskRequest)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var createdSubtaskService = await _subTaskService.CreateSubTaskAsync(subTaskRequest);

            return createdSubtaskService.Succeeded
                ? TypedResults.Ok(createdSubtaskService)
                : TypedResults.BadRequest(createdSubtaskService);
        }

        [HttpPut]
        public async Task<IResult> UpdateSubTask([FromBody] UpdateSubTaskRequestDto subTaskRequest)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var updatedSubtaskService = await _subTaskService.UpdateSubTaskAsync(userId, subTaskRequest);

            return updatedSubtaskService.Succeeded
               ? TypedResults.Ok(updatedSubtaskService)
               : TypedResults.BadRequest(updatedSubtaskService);
        }

        [HttpDelete("{subTaskId}")]
        public async Task<IResult> DeleteSubTask([FromRoute] int subTaskId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var deletedSubtaskService = await _subTaskService.DeleteSubTaskAsync(userId, subTaskId);

            return deletedSubtaskService.Succeeded
               ? TypedResults.Ok(deletedSubtaskService)
               : TypedResults.BadRequest(deletedSubtaskService);
        }
    }
}
