namespace KanbanAppApi.Dtos
{
    public class BoarTaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Position { get; set; }
        public List<SubtaskDto> Subtasks { get; set; } = new();
    }
}
