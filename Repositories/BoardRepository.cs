using KanbanAppApi.Data;
using KanbanAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Repositories;

public class BoardRepository : IBoardRepository
{
    private readonly ApplicationContextDB _context;

    public BoardRepository(ApplicationContextDB context) => _context = context;

    public async Task<Board> CreateAsync(Board board)
    {
        var boardDb = await _context.Boards.AddAsync(board);
        await _context.SaveChangesAsync();
        return boardDb.Entity;
    }

    public async Task DeleteAsync(Board board)
    { 
        _context.Boards.Remove(board);
        await _context.SaveChangesAsync();
    }

    public async Task<Board?> GetByIdAsync(int boardId)
    {
        return await _context.Boards.SingleOrDefaultAsync(x => x.Id == boardId);
    }

    public async Task<IEnumerable<Board>> GetAllByUserId(Guid userId)
    {
        return await _context.Boards
            .Where(b => b.UserId == userId)
            .ToListAsync();;
    }

    public async Task<Board> UpdateAsync(Board board)
    {
        _context.Boards.Update(board);
        await _context.SaveChangesAsync();
        return board;
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, int boardId)
    {
        return await _context.Boards.AnyAsync(b => b.Id == boardId && b.UserId == userId);
    }
}