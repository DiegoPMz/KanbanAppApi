using KanbanAppApi.Dtos;
using KanbanAppApi.Models;
using KanbanAppApi.Repositories;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services
{
    public class ColumnService : IColumnService
    {
        private readonly IColumnRepository _columnRepository;
        private readonly IBoardRepository _boardRepository;

        public ColumnService(IColumnRepository columnRepository, IBoardRepository boardRepository)
        {
            _columnRepository = columnRepository;
            _boardRepository = boardRepository;
        }

        public async Task<ApiResponse<ColumnDto?>> CreateColumnAsync(Guid userId, CreateColumnRequestDto columnRequest)
        {
            if (!await _boardRepository.BoardExistsForUserAsync(userId, columnRequest.BoardId))
            {
                return ApiResponse<ColumnDto?>.Failure("Board not found", []);
            }

            var column = new Column
            {
                BoardId = columnRequest.BoardId,
                Name = columnRequest.Name,
                Color = columnRequest.Color,
            };

            IEnumerable<Column> columnsDb = await _columnRepository.GetColumnsByBoardIdAsync(columnRequest.BoardId);
            column.Position = columnsDb.Count() + 1;

            Column? createdColumn = await _columnRepository.CreateColumnAsync(column);
            if (createdColumn is null) return ApiResponse<ColumnDto?>.Failure("Failed to create column", []);

            var columnResult = new ColumnDto
            {
                Id = createdColumn.Id,
                Name = createdColumn.Name,
                Position = createdColumn.Position,
                Color = createdColumn.Color,
                Tasks = []
            };

            return ApiResponse<ColumnDto?>.Success(columnResult, "Column created successfully");
        }

        public async Task<ApiResponse<object?>> DeleteColumnAsync(Guid userId, int columnId)
        {
            if (!await _columnRepository.ColumnExistsForUserAsync(userId, columnId))
            {
                return ApiResponse<object?>.Failure("Column not found", []);
            }

            var column = await _columnRepository.GetColumnByIdAsync(columnId);
            if (column is null) return ApiResponse<object?>.Failure("Column not found", []);

            await _columnRepository.DeleteColumnAsync(column);
            return ApiResponse<object?>.Success(null, "Column deleted successfully");
        }

        public async Task<ApiResponse<ColumnDto?>> UpdateColumnAsync(Guid userId, UpdateColumnRequestDto columnRequest)
        {
            if (!await _columnRepository.ColumnExistsForUserAsync(userId, columnRequest.Id))
            {
                return ApiResponse<ColumnDto?>.Failure("Column not found", []);
            }

            var columnDb = await _columnRepository.GetColumnByIdAsync(columnRequest.Id)!;
            if (columnDb is null || columnDb.BoardId != columnRequest.BoardId) return ApiResponse<ColumnDto?>.Failure("Column not found", []);

            columnDb.Name = columnRequest.Name ?? columnDb.Name;
            columnDb.Color = columnRequest.Color ?? columnDb.Color;

            return ApiResponse<ColumnDto?>.Success(new ColumnDto
            {
                Id = columnDb.Id,
                Name = columnDb.Name,
                Position = columnDb.Position,
                Color = columnDb.Color,
                Tasks = []
            }, "Column updated successfully");
        }

        public async Task<ApiResponse<List<ColumnPositionDto>?>> ReorderColumnsAsync(Guid userId, ReorderColumnRequestDto reorderRequest)
        {
            if (!await _columnRepository.ColumnExistsForUserAsync(userId, reorderRequest.Id))
            {
                return ApiResponse<List<ColumnPositionDto>?>.Failure("Column not found", []);
            }

            var currentColumn = await _columnRepository.GetColumnByIdAsync(reorderRequest.Id);
            if (currentColumn is null || currentColumn.BoardId != reorderRequest.BoardId)
            {
                return ApiResponse<List<ColumnPositionDto>?>.Failure("Column not found", []);
            }

            IEnumerable<Column> columnsDb = await _columnRepository.GetColumnsByBoardIdAsync(reorderRequest.BoardId);

            if (reorderRequest.Position < 1 || reorderRequest.Position > columnsDb.Count())
            {
                return ApiResponse<List<ColumnPositionDto>?>.Failure(
                    $"Invalid position. Allowed range is from 1 to {columnsDb.Count()}",
                    []
                );
            }

            if (currentColumn.Position == reorderRequest.Position)
            {
                var noChangeResult = columnsDb
                    .Select(c => new ColumnPositionDto { Id = c.Id, Position = c.Position })
                    .ToList();

                return ApiResponse<List<ColumnPositionDto>?>.Success(noChangeResult, "No reordering needed");
            }

            List<Column> reorderedColumns = new();
            int index = 1;

            foreach (var c in columnsDb.Where(c => c.Id != reorderRequest.Id))
            {
                if (index == reorderRequest.Position)
                {
                    currentColumn.Position = reorderRequest.Position;
                    reorderedColumns.Add(currentColumn);
                    index++;
                }

                c.Position = index++;
                reorderedColumns.Add(c);
            }

            if (!reorderedColumns.Contains(currentColumn))
            {
                currentColumn.Position = index;
                reorderedColumns.Add(currentColumn);
            }

            await _columnRepository.UpdateColumnsPositionsAsync(reorderedColumns);
            List<ColumnPositionDto> result = reorderedColumns
                .Select(c => new ColumnPositionDto { Id = c.Id, Position = c.Position })
                .ToList();

            return ApiResponse<List<ColumnPositionDto>?>.Success(result, "Columns reordered successfully");
        }
    }
}
