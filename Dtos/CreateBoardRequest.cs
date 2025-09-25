using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos
{
    public record CreateBoardRequest(
        [Required][MinLength(1)] string Name,
        [Required][MinLength(1)] List<string> Columns
    );
}
