using KanbanAppApi.Data;
using KanbanAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Repositories;

public class BoardTaskRepository : IBoardTaskRepository
{
    private readonly ApplicationContextDb _context;
    public BoardTaskRepository(ApplicationContextDb context) => _context = context;

    public async Task<BoardTask> CreateAsync(BoardTask boardTask)
    {
        var newTask = await _context.BoardTask.AddAsync(boardTask);
        await _context.SaveChangesAsync();
        return newTask.Entity;
    }

    public async Task DeleteAsync(BoardTask boardTask)
    {
        _context.BoardTask.Remove(boardTask);
        await _context.SaveChangesAsync();           
    }

    public async Task<BoardTask?> GetByIdAsync(int boardTaskId)
    {
        return await _context.BoardTask.FirstOrDefaultAsync(bt => bt.Id == boardTaskId);
    }
    
    public async Task<List<BoardTask>> GetByColumnIdAsync(int columnId)
    {
        return await _context.BoardTask
            .Where(bt => bt.ColumnId == columnId)
            .ToListAsync();
    }

    public async Task<int> GetCountByColumnIdAsync(int columnId)
    {
        return await _context.BoardTask
            .Where(bt => bt.ColumnId == columnId)
            .CountAsync();
    }

    public async Task<BoardTask> UpdateAsync(BoardTask boardTask)
    {
        _context.BoardTask.Update(boardTask);
        await _context.SaveChangesAsync();
        return boardTask;
    }

    public async Task UpdatePositionsAsync(List<BoardTask> boardTasks)
    {
        _context.BoardTask.UpdateRange(boardTasks);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UserOwnsBoardTaskAsync(Guid userId, int boardTaskId)
    {
        return await _context.BoardTask
            .AnyAsync(bt => 
                bt.Id == boardTaskId && bt.Column.Board.UserId == userId
            );
    }
}