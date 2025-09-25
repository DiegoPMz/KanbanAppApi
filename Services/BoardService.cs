using KanbanAppApi.Dtos;
using KanbanAppApi.Models;
using KanbanAppApi.Repositories;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRepository _boardRepository;
        private readonly IColumnRepository _columnRepository;

        public BoardService(IBoardRepository boardRepository, IColumnRepository columnRepository)
        {
            _boardRepository = boardRepository;
            _columnRepository = columnRepository;
        }

        public async Task<ApiResponse<BoardDto?>> GetBoardAsync(Guid userId, int boardId)
        {
            var boardEntity = await _boardRepository.GetBoardByIdAsync(boardId);
            if (boardEntity == null || boardEntity.UserId != userId)
                return ApiResponse<BoardDto?>.Failure("Board not found or access denied", []);

            List<ColumnDto> columnDtos = await _columnRepository.GetColumnsWithBoardTasksAndSubtasksAsync(boardEntity.Id);

            var boardDetailsDto = new BoardDto
            {
                Id = boardEntity.Id,
                Name = boardEntity.Name,
                Columns = columnDtos
            };

            return ApiResponse<BoardDto?>.Success(boardDetailsDto, "Board retrieved successfully");
        }

        public async Task<ApiResponse<BoardDto?>> CreateBoardAsync(Guid userId, CreateBoardRequest requestBoard)
        {
            Board newBoard = new(requestBoard.Name, userId);
            requestBoard.Columns.Select((name, index) =>
                new Column
                {
                    Name = name,
                    Position = index
                }
            ).ToList().ForEach(c => newBoard.Columns.Add(c));

            var createdBoard = await _boardRepository.CreateBoardAsync(newBoard);
            if (createdBoard == null) return ApiResponse<BoardDto?>.Failure("Failed to create board", []);

            IEnumerable<Column> createdColumns = await _columnRepository.GetColumnsByBoardIdAsync(createdBoard.Id);
            var createdBoardDto = new BoardDto 
            {  
                Id = createdBoard.Id,
                Name = createdBoard.Name,
                Columns = createdColumns.Select(c => new ColumnDto { 
                    Id =c.Id, 
                    Name=c.Name, 
                    Position = c.Position, 
                    Color = c.Color 
                }).ToList()
            };

            return ApiResponse<BoardDto?>.Success(createdBoardDto,"Board created successfully");
        }

        public async Task<ApiResponse<object?>> DeleteBoardAsync(Guid userId, int boardId)
        {
            var board = await _boardRepository.GetBoardByIdAsync(boardId);
            if (board == null || board.UserId != userId)
                return ApiResponse<object?>.Failure("Board not found or access denied", []);

            await _boardRepository.DeleteBoardAsync(board);
            return ApiResponse<object?>.Success(null, "Board deleted successfully");
        }

        public async Task<ApiResponse<UpdateBoardNameDto?>> UpdateBoardNameAsync(Guid userId, UpdateBoardRequest requestBoard)
        {
            var board = await _boardRepository.GetBoardByIdAsync(requestBoard.boardId);
            if (board == null || board.UserId != userId)
                return ApiResponse<UpdateBoardNameDto?>.Failure("Board not found or access denied", []);

            board.Name = requestBoard.Name;
            var updatedBoard = await _boardRepository.UpdateBoardAsync(board);
            if (updatedBoard == null)
                return ApiResponse<UpdateBoardNameDto?>.Failure("Failed to update board", []);

            var updatedBoardDto = new UpdateBoardNameDto
            {
                Id = updatedBoard.Id,
                Name = updatedBoard.Name,
            };

            return ApiResponse<UpdateBoardNameDto?>.Success(updatedBoardDto, "Board name updated successfully");
        }
    }
}
