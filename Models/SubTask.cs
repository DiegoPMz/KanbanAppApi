using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models
{
    [Table("subTasks")]
    public class SubTask
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }

        public int TaskId { get; set; }
        public Task Task { get; set; } = null!;
    }
}
