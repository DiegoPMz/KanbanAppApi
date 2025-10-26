using KanbanAppApi.Models;

namespace KanbanAppApi.Dtos;

public class BoarTaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Position { get; set; }
    public List<SubtaskDto> Subtasks { get; set; } = [];
        
    public BoarTaskDto() { }
    public BoarTaskDto(BoardTask boardTask)
    {
        Id = boardTask.Id;
        Title = boardTask.Title;
        Description = boardTask.Description;
        Position = boardTask.Position;
        Subtasks = boardTask.SubTasks.Select(s => new SubtaskDto(s)).ToList();
    }
}