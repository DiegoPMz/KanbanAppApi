using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models;

[Table("subTasks")]
public class SubTask
{
    public int Id { get; init; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public int BoardTaskId { get; init; }
    public BoardTask BoardTask { get; init; } = null!;
    
    public SubTask(string description, int boardTaskId)
    {
        Description = description;
        BoardTaskId = boardTaskId;
    }
}