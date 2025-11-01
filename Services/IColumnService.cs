using FluentResults;
using KanbanAppApi.Dtos;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services;

public interface IColumnService
{
    Task<Result<ColumnDto>>  CreateAsync(Guid userId, CreateColumnRequestDto columnRequest);
    Task<Result<string>> DeleteAsync(Guid userId, int columnId);
    Task<Result<UpdateColumnResponseDto>> UpdateAsync(Guid userId, UpdateColumnRequestDto columnRequest);
    Task<Result<List<ColumnPositionDto>>> ReorderColumnsAsync(Guid userId, ReorderColumnRequestDto reorderRequest);
}