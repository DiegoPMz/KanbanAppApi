using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos
{
    public record UpdateBoardTaskRequestDto(
        int Id,
        [MinLength(1)]string? Title,
        string? Description, 
        int? ColumnId, 
        bool? IsCompleted
    );
}
