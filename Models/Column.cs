using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models;

[Table("columns")]
public class Column
{
    public int Id { get; init; }
    public string Name { get; set; }
    public int Position { get; set; } = 1;
    public string Color { get; set; } = string.Empty;
    public List<BoardTask> BoardTask { get; } = [];
    public int BoardId { get; init; }
    public Board Board { get; init; } = null!;
}