using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos
{
    public record ChangeThemeRequest(
        [Required][MinLength(1)] string Theme
    );
}
