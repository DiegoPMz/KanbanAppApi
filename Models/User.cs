using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models
{
    [Table("users")]
    public class User
    {
        public Guid Id { get; set; }
        public string Sub { get; set; }
        public string Email { get; set; }
        public string AppTheme { get; set; }
        public List<Board> Boards { get; } = [];
    }
}
