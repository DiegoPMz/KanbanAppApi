using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models
{
    [Table("tasks")]
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Position { get; set; } = 0;
        public bool IsCompleted { get; set; }
        public List<SubTask> SubTasks { get; } = [];

        public int ColumnId { get; set; }
        public Column Column { get; set; } = null!;
    }
}
