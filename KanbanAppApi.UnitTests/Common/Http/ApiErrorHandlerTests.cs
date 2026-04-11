using ErrorOr;
using KanbanAppApi.Common.Http;
using Microsoft.AspNetCore.Http;

namespace KanbanAppApi.UnitTests.Common.Http;

public class ApiErrorHandlerTests
{
    [Fact]
    public void Problem_ShouldUseErrorCodeAsTitle()
    {
        // Arrange
        var error = Error.Validation(
            code: "User.InvalidEmail",
            description: "The provided email is not valid.");

        // Act
        var result = ApiErrorHandler.Problem(error);
        var problemDetails = result.ProblemDetails;

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal("User.InvalidEmail", problemDetails.Title); 
        Assert.Equal(error.Description, problemDetails.Detail);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
    }

    [Theory]
    [InlineData(ErrorType.Validation, StatusCodes.Status400BadRequest)]
    [InlineData(ErrorType.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(ErrorType.Conflict, StatusCodes.Status409Conflict)]
    [InlineData(ErrorType.Unauthorized, StatusCodes.Status401Unauthorized)]
    [InlineData(ErrorType.Failure, StatusCodes.Status500InternalServerError)]
    public void Problem_ShouldMapCorrectStatusCodeBasedOnErrorType(ErrorType errorType, int expectedStatusCode)
    {
        // Arrange
        var error = Error.Custom((int)errorType, "Test.Code", "Test description");

        // Act
        var result = ApiErrorHandler.Problem(error);

        // Assert
        Assert.Equal(expectedStatusCode, result.StatusCode);
    }

    [Fact]
    public void Problem_ShouldIncludeErrorTypeInExtensions()
    {
        // Arrange
        var error = Error.NotFound("Task.NotFound", "Task not found");

        // Act
        var result = ApiErrorHandler.Problem(error);
        var problemDetails = result.ProblemDetails;

        // Assert
        Assert.NotNull(problemDetails);
        Assert.NotNull(problemDetails.Extensions);
        Assert.True(problemDetails.Extensions.ContainsKey("error_type"));
        Assert.Equal("NotFound", problemDetails.Extensions["error_type"]?.ToString());
    }
    
    [Fact]
    public void Problem_ShouldReturnInternalServerError_WhenErrorTypeIsNotMapped()
    {
        // Arrange
        var error = Error.Custom(
            type : 0000,
            code: "Server.UnexpectedError",
            description :"An unexpected error occurred.",
            metadata: null
        );

        // Act
        var result = ApiErrorHandler.Problem(error);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Equal("Server.UnexpectedError", result.ProblemDetails?.Title);
    }
}