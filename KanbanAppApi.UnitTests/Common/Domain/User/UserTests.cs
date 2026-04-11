using Bogus;
using KanbanAppApi.Common.Domain.User;
using UserModel = KanbanAppApi.Common.Domain.User.User;

namespace KanbanAppApi.UnitTests.Common.Domain.User;

public class UserTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Create_ShouldSucceed_WhenDataIsValid()
    {
        // Arrange
        var externalId = _faker.Random.Guid().ToString();
        var email = _faker.Internet.Email();
        var name = _faker.Name.FirstName();

        // Act
        var result = UserModel.Create(externalId, email, name);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(email.ToLowerInvariant(), result.Value.Email);
        Assert.Equal("Light", result.Value.AppTheme);
        Assert.Equal(result.Value.CreatedAt, result.Value.UpdatedAt);
    }

    [Fact]
    public void Create_ShouldReturnMultipleErrors_WhenMultipleFieldsAreInvalid()
    {
        // Arrange
        var invalidEmail = "correo-no-valido";
        var longName = new string('a', 201);

        // Act
        var result = UserModel.Create(string.Empty, invalidEmail, longName);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == UserErrors.ExternalIdRequired.Code);
        Assert.Contains(result.Errors, e => e.Code == UserErrors.InvalidEmail.Code);
        Assert.Contains(result.Errors, e => e.Code == UserErrors.NameTooLong.Code);
    }

    [Fact]
    public void Create_ShouldTrimAndLowercaseData_WhenInputHasSpacesOrUppercase()
    {
        // Arrange
        var email = "  TEST@Domain.COM  ";
        var name = "  John  ";

        // Act
        var result = UserModel.Create("ext-123", email, name);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal("test@domain.com", result.Value.Email);
        Assert.Equal("John", result.Value.Name);
    }

    [Theory]
    [InlineData("Dark")]
    [InlineData("CustomTheme")]
    public void Create_ShouldApplyAppTheme_WhenProvided(string theme)
    {
        // Act
        var result = UserModel.Create("id", "test@test.com", appTheme: theme);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(theme, result.Value.AppTheme);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenPictureUrlIsTooLong()
    {
        // Arrange
        var longUrl = _faker.Random.AlphaNumeric(1001);

        // Act
        var result = UserModel.Create("id", "test@test.com", pictureUrl: longUrl);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(UserErrors.PictureUrlTooLong.Code, result.FirstError.Code);
    }
    
    [Fact]
    public void Create_ShouldReturnFamilyNameTooLong_WhenFamilyNameExceedsLimit()
    {
        // Arrange
        var longFamilyName = new string('s', 201);

        // Act
        var result = UserModel.Create(
            _faker.Random.Guid().ToString(), 
            "test@test.com", 
            familyName: longFamilyName
        );

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == UserErrors.FamilyNameTooLong.Code);
    }
}