using KanbanAppApi.Models;

namespace KanbanAppApi.Dtos;

public class SubtaskDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
        
    public SubtaskDto() { }
    public SubtaskDto(SubTask subTask)
    {
        Id = subTask.Id;
        Description = subTask.Description;
        IsCompleted = subTask.IsCompleted;
    }
}