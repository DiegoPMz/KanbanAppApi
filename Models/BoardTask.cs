using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models;

[Table("boardTasks")]
public class BoardTask
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Position { get; set; } = 0;
    public bool IsCompleted { get; set; } = false;
    public List<SubTask> SubTasks { get; } = [];

    public int ColumnId { get; set; }
    public Column Column { get; set; } = null!;
}