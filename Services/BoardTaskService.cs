using KanbanAppApi.Dtos;
using KanbanAppApi.Models;
using KanbanAppApi.Repositories;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services
{
    public class BoardTaskService : IBoardTaskService
    {
        private readonly IBoardTaskRepository _boardTaskRespository;
        private readonly IColumnRepository _columnRepository;

        public BoardTaskService(IBoardTaskRepository boardTaskRespository, IColumnRepository columnRepository)
        {
            _boardTaskRespository = boardTaskRespository;
            _columnRepository = columnRepository;
        }

        public async Task<ApiResponse<BoardTaskResponseDto?>> CreateBoardTaskAsync(Guid userId, CreateBoardTaskRequestDto boardTaskRequest)
        {
            if (!await _columnRepository.ColumnExistsForUserAsync(userId, boardTaskRequest.ColumnId))
            {
                return ApiResponse<BoardTaskResponseDto?>.Failure($"Column with {boardTaskRequest.ColumnId} doesnt exist", []);
            }

            var boardTask = new BoardTask
            {
                Title = boardTaskRequest.Title,
                Description = boardTaskRequest.Description ?? "",
                Position = 0,
                IsCompleted = false,
                ColumnId = boardTaskRequest.ColumnId
            };

            BoardTask? boardTaskDb = await _boardTaskRespository.CreateBoardTaskAsync(boardTask);

            if (boardTaskDb == null)
            {
                return ApiResponse<BoardTaskResponseDto?>.Failure("Failed to create board task", []);
            }

            var response = new BoardTaskResponseDto
            {
                Id = boardTaskDb.Id,
                Title = boardTaskDb.Title,
                Description = boardTaskDb.Description,
                Position = boardTaskDb.Position,
                IsCompleted = boardTaskDb.IsCompleted,
                ColumnId = boardTaskDb.ColumnId
            };

            return ApiResponse<BoardTaskResponseDto?>.Success(response, "Board task created successfully");
        }

        public async Task<ApiResponse<object?>> DeleteBoardTaskAsync(Guid userId, int boardTaskId)
        {
           BoardTask? boardTask =  await _boardTaskRespository.GetBoardTaskByIdAsync(boardTaskId);

            if (boardTask == null)
            {
                return ApiResponse<object?>.Failure($"BoardTask with id {boardTaskId} doesnt exist", []);
            }

            if (!await _columnRepository.ColumnExistsForUserAsync(userId, boardTask.ColumnId))
            {
                return ApiResponse<object?>.Failure($"BoardTask with id {boardTaskId} doesnt exist for user", []);
            }

            await _boardTaskRespository.DeleteBoardTask(boardTask);
            return ApiResponse<object?>.Success(null, "Board task deleted successfully");
        }

        public async Task<ApiResponse<BoardTaskResponseDto?>> UpdateBoardTaskAsync(Guid userId, UpdateBoardTaskRequestDto boardTaskRequest)
        {
            BoardTask? boardTask = await _boardTaskRespository.GetBoardTaskByIdAsync(boardTaskRequest.Id);

            if (boardTask == null)
            {
                return ApiResponse<BoardTaskResponseDto?>.Failure($"BoardTask with id {boardTaskRequest.Id} doesnt exist", []);
            }

            if (!await _columnRepository.ColumnExistsForUserAsync(userId, boardTask.ColumnId))
            {
                return ApiResponse<BoardTaskResponseDto?>.Failure($"Column with id {boardTaskRequest.ColumnId} doesnt exist", []);
            }

            boardTask.Title = boardTaskRequest.Title ?? boardTask.Title;
            boardTask.Description = boardTaskRequest.Description ?? boardTask.Description;
            boardTask.IsCompleted = boardTaskRequest.IsCompleted ?? boardTask.IsCompleted;
            boardTask.ColumnId = boardTaskRequest.ColumnId ?? boardTask.ColumnId;

            BoardTask? updatedBoardTask = await _boardTaskRespository.UpdateBoardTaskAsync(boardTask);

            if (updatedBoardTask == null)
            {
                return ApiResponse<BoardTaskResponseDto?>.Failure("Failed to update board task", []);
            }

            var response = new BoardTaskResponseDto
            {
                Id = updatedBoardTask.Id,
                Title = updatedBoardTask.Title,
                Description = updatedBoardTask.Description,
                Position = updatedBoardTask.Position,
                IsCompleted = updatedBoardTask.IsCompleted,
                ColumnId = updatedBoardTask.ColumnId
            };

            return ApiResponse<BoardTaskResponseDto?>.Success(response, "Board task updated successfully");
        }

        public async Task<ApiResponse<List<BoardTaskPositionDto>?>> ReorderBoardTaskAsync(Guid userId, ReorderBoardTaskRequestDto boardTaskRequest)
        {
            BoardTask? boardTask = await _boardTaskRespository.GetBoardTaskByIdAsync(boardTaskRequest.Id);

            if (boardTask == null)
            {
                return ApiResponse<List<BoardTaskPositionDto>?>.Failure($"BoardTask with id {boardTaskRequest.Id} doesnt exist", []);
            }

            if (!await _columnRepository.ColumnExistsForUserAsync(userId, boardTask.ColumnId))
            {
                return ApiResponse<List<BoardTaskPositionDto>?>.Failure($"Column with id {boardTaskRequest.ColumnId} doesnt exist", []);
            }


            IEnumerable<BoardTask> boardTasks = await _boardTaskRespository.GetBoardTasksByColumnIdAsync(boardTaskRequest.ColumnId);

            if (boardTaskRequest.Position < 1 || boardTaskRequest.Position > boardTasks.Count())
            {
                return ApiResponse<List<BoardTaskPositionDto>?>.Failure(
                    $"Invalid position. Allowed range is from 1 to {boardTasks.Count()}",
                    []
                );
            }

            if (boardTask.Position == boardTask.Position)
            {
                var noChangeResult = boardTasks
                    .Select(c => new BoardTaskPositionDto { Id = c.Id, Position = c.Position })
                    .ToList();

                return ApiResponse<List<BoardTaskPositionDto>?>.Success(noChangeResult, "No reordering needed");
            }

            List<BoardTask> reorderedBoardTasks = new();
            int index = 1;

            foreach (var c in boardTasks.Where(c => c.Id != boardTaskRequest.Id))
            {
                if (index == boardTaskRequest.Position)
                {
                    boardTask.Position = boardTaskRequest.Position;
                    reorderedBoardTasks.Add(boardTask);
                    index++;
                }

                c.Position = index++;
                reorderedBoardTasks.Add(c);
            }

            if (!reorderedBoardTasks.Contains(boardTask))
            {
                boardTask.Position = index;
                reorderedBoardTasks.Add(boardTask);
            }

            await _boardTaskRespository.UpdateBoardTasksPositionsAsync(reorderedBoardTasks);
            List<BoardTaskPositionDto> result = reorderedBoardTasks
                .Select(c => new BoardTaskPositionDto { Id = c.Id, Position = c.Position })
                .ToList();

            return ApiResponse<List<BoardTaskPositionDto>?>.Success(result, "Columns reordered successfully");
        }

    }
}
