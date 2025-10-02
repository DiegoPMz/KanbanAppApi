using KanbanAppApi.Dtos;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services
{
    public interface ISubTaskService
    {
        Task<ApiResponse<SubtaskDto?>> CreateSubTaskAsync(CreateSubTaskRequestDto subTaskRequest);
        Task<ApiResponse<SubtaskDto?>> UpdateSubTaskAsync(Guid userId, UpdateSubTaskRequestDto subTaskRequest);
        Task<ApiResponse<SubtaskDto?>> DeleteSubTaskAsync(Guid userId, int subTaskId);
    }
}
