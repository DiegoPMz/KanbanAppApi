using ErrorOr;

namespace KanbanAppApi.Common.Domain.BoardAggregate;

public static class BoardErrors
{
    public static Error NotFound(string identifier) => Error.NotFound(
        code: "BOARD_NOT_FOUND",
        description: $"Board with identifier '{identifier}' could not be found.");

    public static Error InvalidName(string name) => Error.Validation(
        code: "BOARD_INVALID_NAME",
        description: $"The name '{name}' is invalid or does not meet the required format.");

    public static Error NameAlreadyExists(string name) => Error.Conflict(
        code: "BOARD_ALREADY_EXISTS",
        description: $"A board with the name '{name}' already exists for this user.");

    public static Error AccessDenied = Error.Forbidden(
        code: "BOARD_ACCESS_DENIED",
        description: "You do not have permission to access or modify this board.");

    public static Error LimitReached(int maxBoards) => Error.Conflict(
        code: "BOARD_LIMIT_REACHED",
        description: $"The maximum limit of {maxBoards} boards has been reached.");
    
    public static Error InvalidColumnCount => Error.Validation(
        code: "BOARD_COLUMN_COUNT_MISMATCH",
        description: "The number of columns provided does not match the current board configuration.");

    public static Error ColumnNotFoundInBoard => Error.NotFound(
        code: "BOARD_COLUMN_NOT_FOUND",
        description: "One or more columns specified do not belong to this board.");

    public static Error DuplicateColumnOrder => Error.Validation(
        code: "BOARD_DUPLICATE_COLUMN_ORDER",
        description: "Each column must be assigned a unique order position; duplicates are not allowed.");

    public static Error InvalidOrderRange(int max) => Error.Validation(
        code: "BOARD_INVALID_ORDER_RANGE",
        description: $"Column order values must be between 1 and {max}.");
    
    public static Error ColumnNotFound(string id) => Error.NotFound(
        code: "BOARD_COLUMN_NOT_FOUND",
        description: $"Column with identifier '{id}' could not be found in this board.");

    public static Error ColumnNameAlreadyExists(string name) => Error.Conflict(
        code: "BOARD_COLUMN_NAME_ALREADY_EXISTS",
        description: $"A column with the name '{name}' already exists in this board.");

    public static Error ColumnLimitReached(int maxColumns) => Error.Conflict(
        code: "BOARD_COLUMN_LIMIT_REACHED",
        description: $"The maximum limit of {maxColumns} columns for this board has been reached.");
}



