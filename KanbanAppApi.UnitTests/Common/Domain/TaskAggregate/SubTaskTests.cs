using Bogus;
using KanbanAppApi.Common.Domain.TaskAggregate;

namespace KanbanAppApi.UnitTests.Common.Domain.TaskAggregate;

public class SubTaskTests
{
    private const int MaxLengthDescription = 250;
    private readonly Faker _faker = new();
    
    private SubTask CreateValidSubTask() 
        => SubTask.Create(_faker.Lorem.Word(), false).Value;
    
    [Fact]
    public void Create_ShouldSucceed_WhenDataIsValid()
    {
        // Arrange
        var description = _faker.Lorem.Sentence();
        var isCompleted = _faker.Random.Bool();

        // Act
        var result = SubTask.Create(description, isCompleted);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(description, result.Value.Description);
        Assert.Equal(isCompleted, result.Value.IsCompleted);
        Assert.NotEqual(Guid.Empty, result.Value.Id); 
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldReturnError_WhenDescriptionIsInvalid(string invalidDescription)
    {
        // Act
        var result = SubTask.Create(invalidDescription);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(SubTaskErrors.DescriptionRequired.Code, result.FirstError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenDescriptionIsTooLong()
    {
        // Arrange
        var longDescription = _faker.Random.AlphaNumeric(MaxLengthDescription + 1);

        // Act
        var result = SubTask.Create(longDescription);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(SubTaskErrors.DescriptionTooLong(MaxLengthDescription).Code, result.FirstError.Code);
    }

    [Fact]
    public void Create_ShouldDefaultIsCompletedToFalse_WhenNull()
    {
        // Act
        var result = SubTask.Create("Valid description", isCompleted: null);

        // Assert
        Assert.False(result.IsError);
        Assert.False(result.Value.IsCompleted);
    }
    
    [Fact]
    public void Update_ShouldSucceed_WhenChangingAllValues()
    {
        // Arrange
        var subTask = CreateValidSubTask();
        var newDesc = "Updated Description";
        var newStatus = true;
        var initialUpdatedAt = subTask.UpdatedAt;

        Thread.Sleep(1);

        // Act
        var result = subTask.Update(newDesc, newStatus);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(newDesc, subTask.Description);
        Assert.True(subTask.IsCompleted);
        Assert.True(subTask.UpdatedAt > initialUpdatedAt);
    }

    [Fact]
    public void Update_ShouldKeepOldDescription_WhenNewDescriptionIsNull()
    {
        // Arrange
        var subTask = CreateValidSubTask();
        var oldDesc = subTask.Description;

        // Act
        var result = subTask.Update(null, true);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(oldDesc, subTask.Description); // No cambió
        Assert.True(subTask.IsCompleted);           // Sí cambió
    }

    [Fact]
    public void Update_ShouldReturnError_WhenDescriptionIsTooLong()
    {
        // Arrange
        var subTask = CreateValidSubTask();
        var longDesc = new string('a', MaxLengthDescription + 1);

        // Act
        var result = subTask.Update(longDesc, null);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(SubTaskErrors.DescriptionTooLong(MaxLengthDescription).Code, result.FirstError.Code);
    }

    [Fact]
    public void Update_ShouldNotUpdateTimestamp_WhenNoValuesActuallyChange()
    {
        // Arrange
        var subTask = CreateValidSubTask();
        var currentDesc = subTask.Description;
        var currentStatus = subTask.IsCompleted;
        var initialUpdatedAt = subTask.UpdatedAt;

        // Act
        var result = subTask.Update(currentDesc, currentStatus);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(initialUpdatedAt, subTask.UpdatedAt);
    }

    [Fact]
    public void Update_ShouldReturnError_WhenDescriptionIsWhiteSpace()
    {
        // Arrange
        var subTask = CreateValidSubTask();

        // Act
        var result = subTask.Update("   ", null);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(SubTaskErrors.DescriptionRequired.Code, result.FirstError.Code);
    }
    
}