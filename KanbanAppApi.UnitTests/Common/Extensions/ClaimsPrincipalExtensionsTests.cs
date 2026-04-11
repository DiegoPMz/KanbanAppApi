using System.Security.Claims;
using KanbanAppApi.Common.Extensions;

namespace KanbanAppApi.UnitTests.Common.Extensions;

public class ClaimsPrincipalExtensionsTests
{
    private const string UserIdClaimName = "userId";

    [Fact]
    public void GetUserId_ShouldReturnGuid_WhenClaimExistsAndIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claims = new List<Claim> { new(UserIdClaimName, userId.ToString()) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = principal.GetUserId();

        // Assert
        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetUserId_ShouldReturnNull_WhenClaimIsMissing()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        // Act
        var result = principal.GetUserId();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetUserId_ShouldReturnNull_WhenClaimIsNotAGuid()
    {
        // Arrange
        var claims = new List<Claim> { new(UserIdClaimName, "not-a-guid") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        var result = principal.GetUserId();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetRequiredUserId_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => principal.GetRequiredUserId());
    }
}