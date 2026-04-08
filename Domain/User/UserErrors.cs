using ErrorOr;

namespace KanbanAppApi.Domain.User;

public static class UserErrors
{
    public static Error InvalidEmail = Error.Validation(
        code: "USER.INVALID_EMAIL",
        description: "The provided email address is not in a valid format.");

    public static Error ExternalIdRequired = Error.Validation(
        code: "USER.EXTERNAL_ID_REQUIRED",
        description: "The external provider identifier is required.");

    public static Error NameTooLong = Error.Validation(
        code: "USER.NAME_TOO_LONG",
        description: "The name or family name cannot exceed 200 characters.");

    public static Error PictureUrlTooLong = Error.Validation(
        code: "USER.PICTURE_URL_TOO_LONG",
        description: "The picture URL cannot exceed 1000 characters.");
    
    public static Error NotFound = Error.NotFound(
        code: "USER.NOT_FOUND",
        description: "The user with the specified identifier was not found.");

    public static Error AppThemeTooLong = Error.Validation(
        code: "USER.APP_THEME_TOO_LONG",
        description: "The app theme name cannot exceed 50 characters.");
}