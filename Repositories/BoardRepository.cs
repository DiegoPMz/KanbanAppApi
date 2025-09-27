using KanbanAppApi.Data;
using KanbanAppApi.Dtos;
using KanbanAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly ApplicationContextDB _context;

        public BoardRepository(ApplicationContextDB context)
        {
            _context = context;
        }

        public async Task<Board?> CreateBoardAsync(Board board)
        {
            var boardDb = await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();
            return boardDb.Entity;
        }

        public async Task DeleteBoardAsync(Board board)
        { 
            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();
        }

        public async Task<Board?> GetBoardByIdAsync(int boardId)
        {
            return await _context.Boards.SingleOrDefaultAsync(x => x.Id == boardId);
        }

        public async Task<IEnumerable<BoardSummaryDto>> GetBoardSummariesByUserIdAsync(Guid userId)
        {
            IEnumerable<BoardSummaryDto> boards = await _context.Boards
                .Where(b => b.UserId == userId)
                .Select(b => new BoardSummaryDto(b.Id,b.Name))
                .ToListAsync();

            return boards;
        }

        public async Task<Board?> UpdateBoardAsync(Board board)
        {
            _context.Boards.Update(board);
            await _context.SaveChangesAsync();
            return board;
        }

        public async Task<bool> BoardExistsForUserAsync(Guid userId, int boardId)
        {
             return await _context.Boards.AnyAsync(b => b.Id == boardId && b.UserId == userId);
        }
    }
}
