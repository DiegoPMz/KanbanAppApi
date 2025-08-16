using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models
{
    [Table("users")]
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string AppTheme { get; set; }

        public List<Board> Boards { get; } = [];
    }
}
