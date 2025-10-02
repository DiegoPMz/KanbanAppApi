using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models
{
    [Table("tokens")]
    public class TokenEntity
    {
        public Guid Jti { get; set; }
        public Guid UserId { get; set; }
    }
}
