using FluentResults;
using KanbanAppApi.Dtos;
using KanbanAppApi.Errors;
using KanbanAppApi.Models;
using KanbanAppApi.Repositories;

namespace KanbanAppApi.Services;

public class BoardService : IBoardService
{
    private readonly IBoardRepository _boardRepository;
    private readonly IColumnRepository _columnRepository;

    public BoardService(IBoardRepository boardRepository, IColumnRepository columnRepository)
    {
        _boardRepository = boardRepository;
        _columnRepository = columnRepository;
    }

    public async Task<Result<BoardDto>> GetByIdAsync(Guid userId, int boardId)
    {
        var boardDb = await _boardRepository.GetByIdAsync(boardId);
        if (boardDb is null || boardDb.UserId != userId) return BoardErrors.NotFound(boardId.ToString());
        
        var columns = await _columnRepository.GetAllWithBoardTasksAndSubtasksAsync(boardId);
        return new BoardDto()
        {
            Id = boardId,
            Name = boardDb.Name,
            Columns = columns.Select(c=> new ColumnDto(c)).ToList() 
        };
    }
    
    public async Task<Result<BoardDto>> CreateAsync(Guid userId, CreateBoardRequest requestBoard)
    {
        Board newBoard = new(requestBoard.Name, userId);
        requestBoard.Columns.Select((c,index) => new Column 
        { 
            Name = c.Name,
            Color = c.Color,
            Position = index
        }).ToList().ForEach(c => newBoard.Columns.Add(c));

        var createdBoard = await _boardRepository.CreateAsync(newBoard);
        var createdColumns = await _columnRepository.GetAllByBoardIdAsync(createdBoard.Id);

        return new BoardDto 
        {  
            Id = createdBoard.Id,
            Name = createdBoard.Name,
            Columns = createdColumns.Select(c => new ColumnDto(c)).ToList()
        };
    }

    public async Task<Result<string>> DeleteAsync(Guid userId, int boardId)
    {
        var board = await _boardRepository.GetByIdAsync(boardId);
        if (board is null || board.UserId != userId) return BoardErrors.NotFound(boardId.ToString()); 
        
        await _boardRepository.DeleteAsync(board);
        return "Board deleted successfully";
    }

    public async Task<Result<UpdateBoardResponseDto>> UpdateAsync(Guid userId, UpdateBoardRequest requestBoard)
    {
        var boarDb = await _boardRepository.GetByIdAsync(requestBoard.BoardId);
        if (boarDb is null || boarDb.UserId != userId) return BoardErrors.NotFound(requestBoard.BoardId.ToString());
        
        boarDb.Name = requestBoard.Name;
        var updatedBoard = await _boardRepository.UpdateAsync(boarDb);
        
        return new UpdateBoardResponseDto
        {
            Id = updatedBoard.Id,
            Name = updatedBoard.Name,
        };
    }
}