using System.Text.RegularExpressions;
using ErrorOr;
namespace KanbanAppApi.Common.Domain.User;

public partial class User
{
    public Guid Id { get; private init; }
    public string ExternalId { get; private set; } = null!;
    public string Email { get; private init; } = null!;
    public string? Name { get; private set; }
    public string? FamilyName { get; private set; }
    public string? PictureUrl { get; private set; }
    public DateTime CreatedAt { get; private init; }
    public DateTime UpdatedAt { get; private set; }
    public string AppTheme { get; private set; }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailGeneratedRegex();

    private User() { }

    private User(
        string externalId, 
        string email,  
        string? name,
        string? familyName, 
        string? pictureUrl,
        string? appTheme
        )
    {
        Id = Guid.NewGuid();
        AppTheme = appTheme ?? "Light";
        
        ExternalId = externalId;
        Email = email;
        Name = name;
        FamilyName = familyName;
        PictureUrl = pictureUrl;
        
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public static ErrorOr<User> Create(
        string externalId, 
        string email, 
        string? name = null, 
        string? familyName = null, 
        string? pictureUrl = null,
        string? appTheme = null
        )
    {
        List<Error> errors = [];

        if (string.IsNullOrWhiteSpace(externalId))
            errors.Add(UserErrors.ExternalIdRequired);

        if (string.IsNullOrWhiteSpace(email) || !EmailGeneratedRegex().IsMatch(email.Trim()))
            errors.Add(UserErrors.InvalidEmail);

        if (name?.Length > 200) errors.Add(UserErrors.NameTooLong);
        if (familyName?.Length > 200) errors.Add(UserErrors.FamilyNameTooLong);
        if (pictureUrl?.Length > 1000) errors.Add(UserErrors.PictureUrlTooLong);
        if (appTheme?.Length > 50) errors.Add(UserErrors.AppThemeTooLong);
        
        if (errors.Count > 0) return errors;
        
        return new User(
            externalId, 
            email.ToLowerInvariant().Trim(), 
            name?.Trim(), 
            familyName?.Trim(), 
            pictureUrl?.Trim(),
            appTheme
        );
    }
}





