using KanbanAppApi.Dtos;
using KanbanAppApi.Filters;
using KanbanAppApi.Responses;
using KanbanAppApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace KanbanAppApi.Controllers { 
    [Authorize]
    [RequireUserId]
    [ApiController]
    [Route("api/column")]
    public class ColumnController : ControllerBase
    {
        private readonly IColumnService _columnService;

        public ColumnController(IColumnService columnService)
        {
            _columnService = columnService;
        }

        [HttpPost]
        public async Task<
            Results<
                Ok<ApiResponse<ColumnDto?>>,
                BadRequest<ApiResponse<ColumnDto?>>
                >
            > 
            CreateColumn([FromBody] CreateColumnRequestDto requestColumn)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var createdColumnService = await _columnService.CreateColumnAsync(userId,requestColumn);

            return createdColumnService.Succeeded 
                ? TypedResults.Ok(createdColumnService)
                : TypedResults.BadRequest(createdColumnService);
        }

        [HttpDelete("{columnId}")]
        public async Task<
            Results<
                Ok<ApiResponse<object?>>,
                BadRequest<ApiResponse<object?>>
                >
            > 
            DeleteColumn([FromRoute] int columnId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var deletedColumnService = await _columnService.DeleteColumnAsync(userId, columnId);

            return deletedColumnService.Succeeded 
                ? TypedResults.Ok(deletedColumnService)
                : TypedResults.BadRequest(deletedColumnService);
        }

        [HttpPut]
        public async Task<
            Results<
                Ok<ApiResponse<ColumnDto?>>,
                BadRequest<ApiResponse<ColumnDto?>>
                >
            >
            UpdateColumn([FromBody] UpdateColumnRequestDto requestColumn)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var updatedColumnService = await _columnService.UpdateColumnAsync(userId, requestColumn);

            return updatedColumnService.Succeeded 
                ? TypedResults.Ok(updatedColumnService)
                : TypedResults.BadRequest(updatedColumnService);
        }

        [HttpPut("reorder")]
        public async Task<
            Results<
                Ok<ApiResponse<List<ColumnPositionDto>?>>,
                BadRequest<ApiResponse<List<ColumnPositionDto>?>>
                >
            > 
            ReorderColumns([FromBody] ReorderColumnRequestDto requestColumn)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var reorderedColumnsService = await _columnService.ReorderColumnsAsync(userId, requestColumn);

            return reorderedColumnsService.Succeeded 
                ? TypedResults.Ok(reorderedColumnsService)
                : TypedResults.BadRequest(reorderedColumnsService);
        }
    }
}
