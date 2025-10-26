using KanbanAppApi.Models;

namespace KanbanAppApi.Dtos;

public class BoardDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ColumnDto> Columns { get; set; } = [];
        
    public BoardDto() { }
    public BoardDto(Board board)
    {
        Id = board.Id;
        Name = board.Name;
        Columns = board.Columns.Select(c => new ColumnDto(c)).ToList();
    }
}