using KanbanAppApi.Data;
using KanbanAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Repositories
{
    public class ColumnRepository : IColumnRepository
    {
        private readonly ApplicationContextDB _context;
        public ColumnRepository(ApplicationContextDB context )
        {
            _context = context;
        }

        public async Task<Column> CreateColumnAsync(int boardId, Column column)
        {
            column.BoardId = boardId;
            var newColumn = await _context.Columns.AddAsync(column);
            return newColumn.Entity;
        }

        public async System.Threading.Tasks.Task DeleteColumnAsync(int boardId, int columnId)
        {
            var column = await _context.Columns
                .FirstOrDefaultAsync(c => c.Id == columnId && c.BoardId == boardId);
            if (column == null) return;

            _context.Columns.Remove(column);
            await _context.SaveChangesAsync();
        }

        public async Task<Column?> GetColumnByIdAsync(int boardId, int columnId)
        {
            return await _context.Columns
                .FirstOrDefaultAsync(c => c.Id == columnId && c.BoardId == boardId);
        }

        public async Task<IEnumerable<Column>> GetColumnsByBoardIdAsync(int boardId)
        {
            return await _context.Columns
                .Where(c => c.BoardId == boardId)
                .ToListAsync();
        }

        public async Task<Column?> UpdateColumnAsync(int boardId, Column column)
        {
            var existingColumn = await _context.Columns
                .FirstOrDefaultAsync(c => c.Id == column.Id && c.BoardId == boardId);

            if (existingColumn == null) return null;

            existingColumn.Name = column.Name;
            existingColumn.Position = column.Position;
            await _context.SaveChangesAsync();

            return existingColumn;
        }
    }
}
