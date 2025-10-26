using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanAppApi.Models;

[Table("users")]
public class User
{
    public Guid Id { get; init; }
        
    [Column(TypeName = "nvarchar(60)")]
    public string Sub { get; init; }
        
    [Column(TypeName = "nvarchar(255)")]
    public string Email { get; init; }
        
    [Column(TypeName = "nvarchar(80)")]
    public string AppTheme { get; set; } = "light";
        
    public User(string sub, string email)
    {
        Sub = sub;
        Email = email;
    }
}