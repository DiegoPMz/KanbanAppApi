using System.ComponentModel.DataAnnotations.Schema;
using KanbanAppApi.Models.Enums;

namespace KanbanAppApi.Models;

[Table("tasks")]
public class BoardTask
{
    public int Id { get; init; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Position { get; set; } = 1;
    public bool IsCompleted { get; set; } = false;
    public List<SubTask> SubTasks { get; } = [];

    public PriorityType Priority { get; set; }
    public int ColumnId { get; set; }
    public Column Column { get; set; } = null!;
}