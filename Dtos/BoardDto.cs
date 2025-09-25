namespace KanbanAppApi.Dtos
{
    public class BoardDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ColumnDto> Columns { get; set; } = new();
    }
}
