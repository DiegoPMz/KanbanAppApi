using KanbanAppApi.Dtos;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services
{
    public interface IColumnService
    {
        Task<ApiResponse<ColumnDto?>> CreateColumnAsync(Guid userId, CreateColumnRequestDto columnRequest);
        Task<ApiResponse<object?>> DeleteColumnAsync(Guid userId, int columnId);
        Task<ApiResponse<ColumnDto?>> UpdateColumnAsync(Guid userId, UpdateColumnRequestDto columnRequest);
        Task<ApiResponse<List<ColumnPositionDto>?>> ReorderColumnsAsync(Guid userId, ReorderColumnRequestDto reorderRequest);
    }
}
