namespace KanbanAppApi.Dtos
{
    public class BoardTaskResponseDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string Description { get; set; } = String.Empty;
        public int Position { get; set; }
        public bool IsCompleted { get; set; }
        public int ColumnId { get; set; }
    }
}
