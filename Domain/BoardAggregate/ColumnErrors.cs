using ErrorOr;

namespace KanbanAppApi.Domain.BoardAggregate;

public static class ColumnErrors
{
    public static Error InvalidName(string name) => Error.Validation(
        code: "COLUMN_INVALID_NAME",
        description: $"The name '{name}' contains invalid characters or exceeds length limits.");
    
    public static Error InvalidColorFormat(string color) => Error.Validation(
        code: "COLUMN_INVALID_COLOR_FORMAT",
        description: $"The value '{color}' is not a valid hexadecimal color code (e.g., #FFFFFF).");
}