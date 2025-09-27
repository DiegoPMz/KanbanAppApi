using KanbanAppApi.Data;
using KanbanAppApi.Dtos;
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

        public async Task<List<ColumnDto>> GetColumnsWithBoardTasksAndSubtasksAsync(int columnId)
        {
            List<ColumnDto> columnWithTasksAndSubtasks = await _context.Columns
                .Where(c => c.Id == columnId)
                .Select(c => new ColumnDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Position = c.Position,
                    Color = c.Color,
                    Tasks = c.BoardTask.Select(bt => new BoarTaskDto
                    {
                        Id = bt.Id,
                        Title = bt.Title,
                        Description = bt.Description,
                        Position = bt.Position,
                        Subtasks = bt.SubTasks.Select(st => new SubtaskDto
                        {
                            Id = st.Id,
                            Description = st.Description,
                            IsCompleted = st.IsCompleted,
                        }).ToList()
                    })
                    .OrderBy(t => t.Position)
                    .ToList()
                })
                .OrderBy(c => c.Position)
                .ToListAsync();

            return columnWithTasksAndSubtasks!;
        }

        public async Task<Column?> UpdateColumnAsync(Column column)
        {
            _context.Columns.Update(column);
            await _context.SaveChangesAsync();
            return column;
        }

        public async Task UpdateColumnsPositionsAsync(List<Column> columns)
        {
            _context.Columns.UpdateRange(columns);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ColumnExistsForUserAsync(Guid userId, int columnId)
        {
            return _context.Columns
                .Include(c => c.Board)
                .AnyAsync(c => c.Id == columnId && c.Board.UserId == userId);
        }
    }
}
