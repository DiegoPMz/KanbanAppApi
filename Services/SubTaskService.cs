using FluentResults;
using KanbanAppApi.Dtos;
using KanbanAppApi.Errors;
using KanbanAppApi.Models;
using KanbanAppApi.Repositories;

namespace KanbanAppApi.Services;

public class SubTaskService : ISubTaskService
{
    private readonly ISubTaskRepository _subTaskRepository;
    private readonly IBoardTaskRepository _boardTaskRepository;
    public SubTaskService(ISubTaskRepository subTaskRepository, IBoardTaskRepository boardTaskRepository)
    {
        _subTaskRepository = subTaskRepository;
        _boardTaskRepository = boardTaskRepository;
    }

    public async Task<Result<SubtaskDto>> CreateAsync(Guid userId, CreateSubTaskRequestDto subTaskRequest)
    {
        if (!await _boardTaskRepository.UserOwnsBoardTaskAsync(userId, subTaskRequest.BoardTaskId))
            BoardTaskErrors.NotFound(subTaskRequest.BoardTaskId.ToString());
        
        SubTask newSubtask = new(subTaskRequest.Description, subTaskRequest.BoardTaskId);
        var createdSubTask = await _subTaskRepository.CreateAsync(newSubtask);
        return  new SubtaskDto(createdSubTask);
    }

    public async Task<Result<string>> DeleteAsync(Guid userId, int subTaskId)
    {
        if (!await _subTaskRepository.UserOwnsSubTaskAsync(userId, subTaskId))
            return SubtaskErrors.NotFound(subTaskId.ToString());
        
        var subTaskDb = await _subTaskRepository.GetByIdAsync(subTaskId);
        await _subTaskRepository.DeleteAsync(subTaskDb!);
        return "Subtask deleted successfully.";
    }

    public async Task<Result<UpdateSubTaskResponseDto>> UpdateAsync(Guid userId, UpdateSubTaskRequestDto subTaskRequest)
    {
        if (!await _subTaskRepository.UserOwnsSubTaskAsync(userId, subTaskRequest.Id))
            return SubtaskErrors.NotFound(subTaskRequest.Id.ToString());

        var subTaskDb = await _subTaskRepository.GetByIdAsync(subTaskRequest.Id);
        if (subTaskDb is null || subTaskDb.BoardTaskId != subTaskRequest.boardTaskId ) 
            return SubtaskErrors.NotFound(subTaskRequest.Id.ToString());

        subTaskDb.Description = subTaskRequest.Description ?? subTaskDb.Description;
        subTaskDb.IsCompleted = subTaskRequest.IsCompleted ?? subTaskDb.IsCompleted;

        var updatedSubTask = await _subTaskRepository.UpdateAsync(subTaskDb);
        return new UpdateSubTaskResponseDto(updatedSubTask.Description, updatedSubTask.IsCompleted);
    }
}