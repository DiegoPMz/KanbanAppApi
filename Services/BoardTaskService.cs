using FluentResults;
using KanbanAppApi.Dtos;
using KanbanAppApi.Errors;
using KanbanAppApi.Models;
using KanbanAppApi.Repositories;

namespace KanbanAppApi.Services;

public class BoardTaskService : IBoardTaskService
{
    private readonly IBoardTaskRepository _boardTaskRepository;
    private readonly IColumnRepository _columnRepository;

    public BoardTaskService(IBoardTaskRepository boardTaskRepository, IColumnRepository columnRepository)
    {
        _boardTaskRepository = boardTaskRepository;
        _columnRepository = columnRepository;
    }

    public async Task<Result<BoarTaskDto>> CreateAsync(Guid userId, CreateBoardTaskRequestDto boardTaskRequest)
    {
        if (!await _columnRepository.ExistsForUserAsync(userId, boardTaskRequest.ColumnId)) 
            return ColumnErrors.NotFound(boardTaskRequest.ColumnId.ToString());
        
        var boarTaskCount = await _boardTaskRepository.GetCountByColumnIdAsync(boardTaskRequest.ColumnId);
        var boardTask = new BoardTask
        {
            Title = boardTaskRequest.Title,
            Description = boardTaskRequest.Description ?? "",
            Position = boarTaskCount + 1,
            IsCompleted = false,
            ColumnId = boardTaskRequest.ColumnId,
            Priority = boardTaskRequest.Priority,
        };

        var boardTaskCreated = await _boardTaskRepository.CreateAsync(boardTask);
        return new BoarTaskDto(boardTaskCreated);
    }

    public async Task<Result<string>> DeleteAsync(Guid userId, int boardTaskId)
    {
        var boardTaskDb =  await _boardTaskRepository.GetByIdAsync(boardTaskId);
        if (boardTaskDb is null) return BoardTaskErrors.NotFound(boardTaskId.ToString());

        if (!await _columnRepository.ExistsForUserAsync(userId, boardTaskDb.ColumnId))
            return BoardTaskErrors.NotFound(boardTaskId.ToString());
        
        await _boardTaskRepository.DeleteAsync(boardTaskDb);
        return "Task deleted successfully";
    }

    public async Task<Result<UpdateBoardTaskResponseDto>> UpdateAsync(Guid userId, UpdateBoardTaskRequestDto boardTaskRequest)
    {
        var boardTaskDb = await _boardTaskRepository.GetByIdAsync(boardTaskRequest.Id);
        if (boardTaskDb is null) return BoardTaskErrors.NotFound(boardTaskRequest.Id.ToString());
        
        if (!await _columnRepository.ExistsForUserAsync(userId, boardTaskDb.ColumnId))
            return BoardTaskErrors.NotFound(boardTaskRequest.Id.ToString());
        
        boardTaskDb.Title = boardTaskRequest.Title ?? boardTaskDb.Title;
        boardTaskDb.Description = boardTaskRequest.Description ?? boardTaskDb.Description;
        boardTaskDb.IsCompleted = boardTaskRequest.IsCompleted ?? boardTaskDb.IsCompleted;
        boardTaskDb.Priority = boardTaskRequest.Priority ?? boardTaskDb.Priority;
        boardTaskDb.ColumnId = boardTaskRequest.ColumnId ?? boardTaskDb.ColumnId;
        
        await _boardTaskRepository.UpdateAsync(boardTaskDb);
        return new UpdateBoardTaskResponseDto(
            Id: boardTaskDb.Id,
            Title: boardTaskDb.Title,
            Description: boardTaskDb.Description,
            ColumnId: boardTaskDb.ColumnId,
            IsCompleted: boardTaskDb.IsCompleted,
            Priority: boardTaskDb.Priority
        );
    }

    public async Task<Result<List<BoardTaskPositionDto>>> ReorderBoardTasksAsync(Guid userId, ReorderBoardTaskRequestDto boardTaskRequest)
    {
        var boardTaskDb = await _boardTaskRepository.GetByIdAsync(boardTaskRequest.Id);
        if (boardTaskDb is null) return BoardTaskErrors.NotFound(boardTaskRequest.Id.ToString());
        
        if (!await _columnRepository.ExistsForUserAsync(userId, boardTaskDb.ColumnId))
            return BoardTaskErrors.NotFound(boardTaskRequest.Id.ToString());
        
        var boardTasks = await _boardTaskRepository.GetByColumnIdAsync(boardTaskRequest.ColumnId);

        if (boardTaskRequest.Position < 1 || boardTaskRequest.Position > boardTasks.Count)
            return BoardTaskErrors.InvalidPosition(boardTasks.Count);

        if (boardTaskDb.Position == boardTaskRequest.Position) return boardTasks
            .Select(c => new BoardTaskPositionDto { Id = c.Id, Position = c.Position })
            .ToList();
        
        List<BoardTask> reorderedBoardTasks = [];
        var index = 1;

        foreach (var c in boardTasks.Where(c => c.Id != boardTaskRequest.Id))
        {
            if (index == boardTaskRequest.Position)
            {
                boardTaskDb.Position = boardTaskRequest.Position;
                reorderedBoardTasks.Add(boardTaskDb);
                index++;
            }

            c.Position = index++;
            reorderedBoardTasks.Add(c);
        }

        if (!reorderedBoardTasks.Contains(boardTaskDb))
        {
            boardTaskDb.Position = index;
            reorderedBoardTasks.Add(boardTaskDb);
        }

        await _boardTaskRepository.UpdatePositionsAsync(reorderedBoardTasks);
        return reorderedBoardTasks
            .Select(c => new BoardTaskPositionDto { Id = c.Id, Position = c.Position })
            .ToList();
    }

}