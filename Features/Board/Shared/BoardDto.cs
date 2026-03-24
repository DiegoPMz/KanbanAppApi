namespace KanbanAppApi.Features.Board.Shared;

public record BoardDto(Guid Id, string Name)
{
    public static BoardDto FromEntity(Domain.BoardAggregate.Board board) => new BoardDto(board.Id, board.Name);
};
