using KanbanAppApi.Data;
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

        public async Task<Board> CreateBoardAsync(int userId, Board board)
        {
            board.UserId = userId;
            var newBoard = await _context.Boards.AddAsync(board);

            return newBoard.Entity;
        }

        public async System.Threading.Tasks.Task DeleteBoardAsync(int userId, int boardId)
        {
            var board = await _context.Boards
                .FirstOrDefaultAsync(b => b.Id == boardId && b.UserId == userId);

            if (board == null) return;    

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync(); 
        }

        public async Task<IEnumerable<Board>> GetAllBoardsAsync(int userId)
        {
           return await _context.Boards
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<Board?> GetBoardByIdAsync(int userId, int boardId)
        {
            return await _context.Boards
                .FirstOrDefaultAsync(b => b.Id == boardId && b.UserId == userId);
        }

        public async Task<Board> UpdateBoardAsync(int userId, Board board)
        {
            var existingBoard = _context.Boards
                .FirstOrDefault(b => b.Id == board.Id && b.UserId == userId);
            if (existingBoard == null)
            {
                throw new KeyNotFoundException("Board not found or does not belong to the user.");
            }

            existingBoard.Name = board.Name;
            await _context.SaveChangesAsync();
            return existingBoard;
        }

    }
}
