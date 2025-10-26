using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models;

[Table("boards")]
public class Board
{
    public int Id { get; init; }
    
    [Column(TypeName = "nvarchar(200)")]
    public string Name { get; set; }
    public List<Column> Columns { get; } = [];

    public Guid UserId { get; init; }
    public User User { get; init; } = null!;

    public Board(string name, Guid userId)
    {
        Name = name;
        UserId = userId;
    }
}