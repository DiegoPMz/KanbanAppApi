using KanbanAppApi.Models;

namespace KanbanAppApi.Dtos
{
    public record UserProfileDto(
        Guid Id,
        string Email,
        string AppTheme,
        List<BoardSummaryDto> Boards
    );
}
