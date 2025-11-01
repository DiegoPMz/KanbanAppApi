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
[Route("api/columns")]
public class ColumnController : ControllerBase
{
    private readonly IColumnService _columnService;
    public ColumnController(IColumnService columnService) => _columnService = columnService;

    [HttpPost]
    [ProducesResponseType(typeof(ColumnDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateColumn([FromBody] CreateColumnRequestDto requestColumn)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var createdColumnResult = await _columnService.CreateAsync(userId,requestColumn);

        return createdColumnResult.IsSuccess
            ? Ok(createdColumnResult.Value)
            : Problem(
                title: createdColumnResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: createdColumnResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }
    
    [HttpDelete("{columnId:int}")]
    [ProducesResponseType(typeof(string),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteColumn([FromRoute] int columnId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var deletedColumnResult = await _columnService.DeleteAsync(userId, columnId);

        return deletedColumnResult.IsSuccess 
            ? Ok(deletedColumnResult.Value)
            : Problem(
                title: deletedColumnResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: deletedColumnResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }

    [HttpPut]
    [ProducesResponseType(typeof(UpdateColumnResponseDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateColumn([FromBody] UpdateColumnRequestDto requestColumn)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var updatedColumnResult = await _columnService.UpdateAsync(userId, requestColumn);

        return updatedColumnResult.IsSuccess 
            ? Ok(updatedColumnResult.Value)
            : Problem(
                title: updatedColumnResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: updatedColumnResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }

    [HttpPut("reorder")]
    [ProducesResponseType(typeof(List<ColumnPositionDto>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReorderColumns([FromBody] ReorderColumnRequestDto requestColumn)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var reorderedColumnsResult = await _columnService.ReorderColumnsAsync(userId, requestColumn);

        return reorderedColumnsResult.IsSuccess 
            ? Ok(reorderedColumnsResult.Value)
            : Problem(
                title: reorderedColumnsResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: reorderedColumnsResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }
}