using KanbanAppApi.Dtos;
using KanbanAppApi.Models;
using KanbanAppApi.Repositories;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services
{
    public class SubTaskService : ISubTaskService
    {
        private readonly ISubTaskRepository _subTaskRepository;

        public SubTaskService(ISubTaskRepository subTaskRepository)
        {
            _subTaskRepository = subTaskRepository;
        }

        public async Task<ApiResponse<SubtaskDto?>> CreateSubTaskAsync(CreateSubTaskRequestDto subTaskRequest)
        {
            SubTask newSubtask = new()
            {
                Description = subTaskRequest.Description,
                IsCompleted = false,
                BoardTaskId = subTaskRequest.TaskId
            };

            var createdSubTask = await _subTaskRepository.CreateSubTaskAsync(newSubtask);
            if (createdSubTask is null)
            {
                return ApiResponse<SubtaskDto?>.Failure("Failed to create subtask.", []);
            }

            var response = new SubtaskDto
            {
                Description = createdSubTask.Description,
                IsCompleted = createdSubTask.IsCompleted,
                Id = createdSubTask.Id
            };

            return ApiResponse<SubtaskDto?>.Success(response, "Subtask created successfully.");
        }

        public async Task<ApiResponse<SubtaskDto?>> DeleteSubTaskAsync(Guid userId, int subTaskId)
        {
            if (!await _subTaskRepository.SubTaskExistsByUserIdAsync(userId,subTaskId))
            {
                return ApiResponse<SubtaskDto?>.Failure("Subtask not found or access denied.", []);
            }

            var subTask = await _subTaskRepository.GetSubTaskByIdAsync(subTaskId);
            if (subTask is null)
            {
                return ApiResponse<SubtaskDto?>.Failure("Subtask not found or access denied.", []);
            }

            await _subTaskRepository.DeleteSubTask(subTask);
            return ApiResponse<SubtaskDto?>.Success(null, "Subtask deleted successfully.");
        }

        public async Task<ApiResponse<SubtaskDto?>> UpdateSubTaskAsync(Guid userId, UpdateSubTaskRequestDto subTaskRequest)
        {
            if (!await _subTaskRepository.SubTaskExistsByUserIdAsync(userId, subTaskRequest.Id))
            {
                return ApiResponse<SubtaskDto?>.Failure("Subtask not found or access denied.", []);
            }

            var subTask = await _subTaskRepository.GetSubTaskByIdAsync(subTaskRequest.Id);
            if (subTask is null)
            {
                return ApiResponse<SubtaskDto?>.Failure("Subtask not found or access denied.", []);
            }

            subTask.Description = subTaskRequest.Description ?? subTask.Description;
            subTask.IsCompleted = subTaskRequest.IsCompleted ?? subTask.IsCompleted;

            var updatedSubTask = await _subTaskRepository.UpdateSubTaskAsync(subTask);
            if (updatedSubTask is null)
            {
                return ApiResponse<SubtaskDto?>.Failure("Failed to update subtask.", []);
            }

            var response = new SubtaskDto
            {
                Description = updatedSubTask.Description,
                IsCompleted = updatedSubTask.IsCompleted,
                Id = updatedSubTask.Id
            };

            return ApiResponse<SubtaskDto?>.Success(response, "Subtask updated successfully.");
        }
    }
}
