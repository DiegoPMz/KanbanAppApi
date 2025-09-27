using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos
{
    public record UpdateColumnRequestDto(
        int BoardId,
        int Id,
        [MinLength(1)] string? Name,
        [MinLength(1)] string? Color
    );
    
}
