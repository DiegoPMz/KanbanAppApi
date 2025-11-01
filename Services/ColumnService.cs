using System.Text.Json;
using KanbanAppApi.Dtos;
using KanbanAppApi.Errors;
using KanbanAppApi.Models;
using KanbanAppApi.Repositories;
using FluentResults;

namespace KanbanAppApi.Services;

public class ColumnService : IColumnService
{
    private readonly IColumnRepository _columnRepository;
    private readonly IBoardRepository _boardRepository;

    public ColumnService(IColumnRepository columnRepository, IBoardRepository boardRepository)
    {
        _columnRepository = columnRepository;
        _boardRepository = boardRepository;
    }

    public async Task<Result<ColumnDto>> CreateAsync(Guid userId, CreateColumnRequestDto columnRequest)
    {
        if (!await _boardRepository.ExistsForUserAsync(userId, columnRequest.BoardId)) 
            return BoardErrors.NotFound(columnRequest.BoardId.ToString());

        Column newColumn = new()
        {
            BoardId = columnRequest.BoardId,
            Name = columnRequest.Name,
            Color = columnRequest.Color,
        };

        var columnsDb = await _columnRepository.GetAllByBoardIdAsync(columnRequest.BoardId);
        newColumn.Position = columnsDb.Count + 1;

        var createdColumn = await _columnRepository.CreateAsync(newColumn);
        Console.WriteLine(JsonSerializer.Serialize(createdColumn));
        return new ColumnDto(createdColumn);
    }

    public async Task<Result<string>> DeleteAsync(Guid userId, int columnId)
    {
        if (!await _columnRepository.ExistsForUserAsync(userId, columnId)) 
            return ColumnErrors.NotFound(columnId.ToString());

        var column = await _columnRepository.GetByIdAsync(columnId);
        await _columnRepository.DeleteAsync(column!);
        return "Column deleted successfully";
    }

    public async Task<Result<UpdateColumnResponseDto>> UpdateAsync(Guid userId, UpdateColumnRequestDto columnRequest)
    {
        if (!await _columnRepository.ExistsForUserAsync(userId, columnRequest.Id))
            return ColumnErrors.NotFound(columnRequest.Id.ToString());

        var columnDb = await _columnRepository.GetByIdAsync(columnRequest.Id)!;
        if (columnDb is null || columnDb.BoardId != columnRequest.BoardId) return ColumnErrors.NotFound(columnRequest.Id.ToString());

        columnDb.Name = columnRequest.Name ?? columnDb.Name;
        columnDb.Color = columnRequest.Color ?? columnDb.Color;

        await _columnRepository.UpdateAsync(columnDb);
        return new UpdateColumnResponseDto(columnDb.Name,  columnDb.Color);
    }

    public async Task<Result<List<ColumnPositionDto>>> ReorderColumnsAsync(Guid userId, ReorderColumnRequestDto reorderRequest)
    {
        if (!await _columnRepository.ExistsForUserAsync(userId, reorderRequest.Id))
            return ColumnErrors.NotFound(reorderRequest.Id.ToString());

        var currentColumn = await _columnRepository.GetByIdAsync(reorderRequest.Id);
        if (currentColumn is null || currentColumn.BoardId != reorderRequest.BoardId)
            return ColumnErrors.NotFound(reorderRequest.Id.ToString());

        var columnsDb = await _columnRepository.GetAllByBoardIdAsync(reorderRequest.BoardId);
        if (reorderRequest.Position < 1 || reorderRequest.Position > columnsDb.Count)
            return ColumnErrors.InvalidPosition(columnsDb.Count);
        
        if (currentColumn.Position == reorderRequest.Position) return columnsDb
            .Select(c => new ColumnPositionDto { Id = c.Id, Position = c.Position })
            .ToList();
        
        List<Column> reorderedColumns = [];
        var index = 1;
        
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

        await _columnRepository.UpdatePositionsAsync(reorderedColumns);
        return reorderedColumns
            .Select(c => new ColumnPositionDto { Id = c.Id, Position = c.Position })
            .ToList();
    }
}