using KanbanAppApi.Data;
using KanbanAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Repositories;

public class SubTaskRepository : ISubTaskRepository
{
    private readonly ApplicationContextDb _context;
    public SubTaskRepository(ApplicationContextDb context) => _context = context;

    public async Task<SubTask> CreateAsync(SubTask subTask)
    {
        var newSubTask = await _context.Subtasks.AddAsync(subTask);
        await _context.SaveChangesAsync();
        return newSubTask.Entity;
    }

    public async Task DeleteAsync(SubTask subTask)
    {
        _context.Subtasks.Remove(subTask);
        await _context.SaveChangesAsync();
    }

    public async Task<SubTask?> GetByIdAsync(int subTaskId)
    {
        return await _context.Subtasks
            .FirstOrDefaultAsync(st => st.Id == subTaskId);
    }

    public async Task<IEnumerable<SubTask>> GetAllByBoardTaskIdAsync(int boardTaskId)
    {
        return await _context.Subtasks
            .Where(st => st.BoardTaskId == boardTaskId)
            .ToListAsync();
    }
    
    public async Task<SubTask> UpdateAsync(SubTask subTask)
    {
        _context.Subtasks.Update(subTask);
        await _context.SaveChangesAsync();
        return subTask;
    }

    public async Task<bool> UserOwnsSubTaskAsync(Guid userId, int  subTaskId)
    { 
        return await _context.Subtasks
            .AnyAsync(
                st => st.Id == subTaskId && st.BoardTask.Column.Board.UserId == userId
            );
    }
}