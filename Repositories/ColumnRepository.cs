using KanbanAppApi.Data;
using KanbanAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Repositories;

public class ColumnRepository : IColumnRepository
{
    private readonly ApplicationContextDb _context;
    public ColumnRepository(ApplicationContextDb context) => _context = context;

    public async Task<Column> CreateAsync(Column column)
    {
        var newColumn = await _context.Columns.AddAsync(column);
        await _context.SaveChangesAsync();
        
        return newColumn.Entity;
    }

    public async Task DeleteAsync(Column column)
    {
        _context.Columns.Remove(column);
        await _context.SaveChangesAsync();
    }

    public async Task<Column?> GetByIdAsync(int columnId)
    {
        return await _context.Columns
            .FirstOrDefaultAsync(c => c.Id == columnId);
    }

    public async Task<List<Column>> GetAllByBoardIdAsync(int boardId)
    {
        return await _context.Columns
            .Where(c => c.BoardId == boardId)
            .OrderBy(c => c.Position)
            .ToListAsync();
    }
    
    public async Task<List<Column>> GetAllWithBoardTasksAndSubtasksAsync(int boardId)
    {
       return await _context.Columns
           .Where(c => c.BoardId == boardId)
           .Include(c => c.BoardTask)
           .ThenInclude(bt => bt.SubTasks)
           .OrderBy(c => c.Position)
           .ToListAsync();
    }
    
    public async Task<Column> UpdateAsync(Column column)
    {
        _context.Columns.Update(column);
        await _context.SaveChangesAsync();
        return column;
    }

    public async Task UpdatePositionsAsync(List<Column> columns)
    {
        _context.Columns.UpdateRange(columns);
        await _context.SaveChangesAsync();
    }

    public Task<bool> ExistsForUserAsync(Guid userId, int columnId)
    {
        return _context.Columns
            .Include(c => c.Board)
            .AnyAsync(c => c.Id == columnId && c.Board.UserId == userId);
    }
}