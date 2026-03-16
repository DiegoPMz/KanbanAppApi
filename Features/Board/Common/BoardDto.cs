
namespace KanbanAppApi.Features.Board.Common;

public record BoardDto(Guid Id, string Name)
{
    public static BoardDto FromEntity(Core.Entities.Board board) => new BoardDto(board.Id, board.Name);
};
