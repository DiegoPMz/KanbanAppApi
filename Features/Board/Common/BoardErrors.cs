using KanbanAppApi.Core.Errors;

namespace KanbanAppApi.Features.Board.Common;

public static class BoardErrors
{
    private static AppError BaseBoardError(string message, string code, SeverityValues severity, string source)
    {
        return new AppError(message)
            .WithCode(code)
            .WithSeverity(severity)
            .WithSource(source)
            .WithUserFacing(true); 
    }

    public static AppError NotFound(string identifier, string source) => 
        BaseBoardError($"Board '{identifier}' not found.", "BOARD_NOT_FOUND", SeverityValues.Warning, source)
            .WithRetryable(false);

    public static AppError InvalidName(string name,string source) => 
        BaseBoardError($"The name '{name}' is invalid.", "BOARD_INVALID_NAME", SeverityValues.Warning ,source)
            .WithRetryable(true);
}