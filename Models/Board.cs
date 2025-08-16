using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models
{
    [Table("boards")]
    public class Board
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Column> Columns { get; } = [];

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
