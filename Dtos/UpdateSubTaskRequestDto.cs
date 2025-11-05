using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos
{
    public record UpdateSubTaskRequestDto(
        int Id,
        int boardTaskId,
        [MinLength(1)] string? Description, 
        bool? IsCompleted
    );
}
