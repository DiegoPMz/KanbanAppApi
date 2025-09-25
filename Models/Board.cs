using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models
{
    [Table("boards")]
    public class Board
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Column> Columns { get; } = [];

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Board() { }
        public Board(string name, Guid userId)
        {
            Name = name;
            UserId = userId;
        }
    }
}
