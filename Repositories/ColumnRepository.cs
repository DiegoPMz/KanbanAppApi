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

        public async Task<Column?> CreateColumnAsync(Column column)
        {
            var newColumn = await _context.Columns.AddAsync(column);
            return newColumn.Entity;
        }

        public async Task DeleteColumnAsync(Column column)
        {
            _context.Columns.Remove(column);
            await _context.SaveChangesAsync();
        }

        public async Task<Column?> GetColumnByIdAsync(int columnId)
        {
            return await _context.Columns
               .FirstOrDefaultAsync(c => c.Id == columnId);
        }

        public async Task<IEnumerable<Column>> GetColumnsByBoardIdAsync(int boardId)
        {
            return await _context.Columns
                .Where(c => c.BoardId == boardId)
                .ToListAsync();
        }

        public async Task<Column?> UpdateColumnAsync(Column column)
        {
            _context.Columns.Update(column);
            await _context.SaveChangesAsync();
            return column;
        }
    }
}
