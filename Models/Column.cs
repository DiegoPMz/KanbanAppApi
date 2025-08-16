using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models
{
    [Table("columns")]
    public class Column
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; } = 0;
        public List<Task> Tasks { get; } = [];

        public int BoardId { get; set; }
        public Board Board { get; set; } = null!;
    }
}
