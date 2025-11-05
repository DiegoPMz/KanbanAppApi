using FluentResults;
using KanbanAppApi.Dtos;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services;

public interface ISubTaskService
{
    Task<Result<SubtaskDto>> CreateAsync(Guid userId, CreateSubTaskRequestDto subTaskRequest);
    Task<Result<UpdateSubTaskResponseDto>> UpdateAsync(Guid userId, UpdateSubTaskRequestDto subTaskRequest);
    Task<Result<string>> DeleteAsync(Guid userId, int subTaskId);
}