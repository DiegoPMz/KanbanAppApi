using KanbanAppApi.Models;

namespace KanbanAppApi.Dtos;

public class ColumnDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }
    public string Color { get; set; }
    public IEnumerable<BoarTaskDto> BoardTask { get; set; } = [];

    public ColumnDto() { }
    public ColumnDto(Column column)
    {
        Id = column.Id;
        Name = column.Name;
        Position = column.Position;
        Color = column.Color;
        BoardTask = column.BoardTask.Select(t => new BoarTaskDto(t)).ToList();
    }
}