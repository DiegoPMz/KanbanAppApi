using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos
{
    public record UpdateSubTaskRequestDto(
        int Id,
        int taskId,
        [MinLength(1)] string? Description, 
        bool? IsCompleted
    );
}
