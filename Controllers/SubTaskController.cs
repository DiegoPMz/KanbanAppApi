using KanbanAppApi.Dtos;
using KanbanAppApi.Errors;
using KanbanAppApi.Filters;
using KanbanAppApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KanbanAppApi.Controllers;

[Authorize]
[RequireUserId]
[ApiController]
[Route("api/subTasks")]
public class SubTaskController : ControllerBase
{
    private readonly ISubTaskService _subTaskService;
    public SubTaskController(ISubTaskService subTaskService) => _subTaskService = subTaskService;

    [HttpPost]
    [ProducesResponseType(typeof(SubtaskDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSubTask([FromBody] CreateSubTaskRequestDto subTaskRequest)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var createdSubtaskResult = await _subTaskService.CreateAsync(userId, subTaskRequest);

        return createdSubtaskResult.IsSuccess
            ? Ok(createdSubtaskResult.Value)
            : Problem(
                title: createdSubtaskResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: createdSubtaskResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }

    [HttpPut]
    [ProducesResponseType(typeof(UpdateSubTaskResponseDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateSubTask([FromBody] UpdateSubTaskRequestDto subTaskRequest)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var updatedSubtaskResult = await _subTaskService.UpdateAsync(userId, subTaskRequest);

        return updatedSubtaskResult.IsSuccess
            ? Ok(updatedSubtaskResult.Value)
            : Problem(
                title: updatedSubtaskResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: updatedSubtaskResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }

    [HttpDelete("{subTaskId:int}")]
    [ProducesResponseType(typeof(string),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubTask([FromRoute] int subTaskId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var deletedSubtaskResult = await _subTaskService.DeleteAsync(userId, subTaskId);

        return deletedSubtaskResult.IsSuccess
            ? Ok(deletedSubtaskResult.Value)
            : Problem(
                title: deletedSubtaskResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: deletedSubtaskResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }
}