using KanbanAppApi.Domain.BoardAggregate;

namespace KanbanAppApi.Features.Board.Shared;

public record ColumnDto(
    Guid Id,
    string Name,
    int Order,
    string Color,
    DateTime CreatedAt,
    DateTime UpdatedAt
)
{
    public static ColumnDto FromEntity(Column column) => new ColumnDto(
        column.Id, 
        column.Name, 
        column.Order, 
        column.Color, 
        column.CreatedAt, 
        column.UpdatedAt
    );
};