using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos
{
    public record CreateSubTaskRequestDto(
        [Required][MinLength(0)] string Description,
        int BoardTaskId
    );
}
