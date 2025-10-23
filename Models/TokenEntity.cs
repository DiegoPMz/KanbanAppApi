using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models
{
    [Table("tokens")]
    public class TokenEntity
    {
        public TokenEntity(Guid jti, Guid userId)
        {
            Jti = jti;
            UserId = userId;
        }

        public TokenEntity() { }

        public Guid Jti { get; set; }
        public Guid UserId { get; set; }
    }
}
