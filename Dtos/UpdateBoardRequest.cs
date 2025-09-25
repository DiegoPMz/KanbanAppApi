using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos
{
    public record UpdateBoardRequest(
        [Required] int boardId,
        [Required][MinLength(1)] string Name
    );
    
}
