namespace KanbanAppApi.Dtos
{
    public class ColumnDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Position { get; set; }
        public string Color { get; set; }   
        public List<BoarTaskDto> Tasks { get; set; } = new();
    }
}
