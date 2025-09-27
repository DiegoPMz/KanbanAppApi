using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos
{
    public record CreateColumnRequestDto(
         int BoardId,
        [Required][MinLength(1)] string Name,
        [Required][MinLength(1)] string Color
    );
}
