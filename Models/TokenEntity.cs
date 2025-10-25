using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models;

[Table("tokens")]
public class TokenEntity
{
    public Guid Jti { get; init; }
    public Guid UserId { get; init; }
    
    public TokenEntity() { }
    public TokenEntity(Guid jti, Guid userId)
    {
        Jti = jti;
        UserId = userId;
    }
}