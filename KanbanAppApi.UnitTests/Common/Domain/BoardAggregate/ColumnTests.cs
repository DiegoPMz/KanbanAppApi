using KanbanAppApi.Common.Domain.BoardAggregate;

namespace KanbanAppApi.UnitTests.Common.Domain.BoardAggregate;

public class ColumnTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldReturnError_WhenNameIsInvalid(string invalidName)
    {
        // Act
        var result = Column.Create(invalidName, 1);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ColumnErrors.InvalidName(invalidName).Code, result.FirstError.Code);
    }
    
    [Fact]
    public void Create_ShouldReturnColumn_WhenDataIsValid()
    {
        // Arrange
        const string name = "Doing";
        const int order = 2;
        const string color = "#FF5733";

        // Act
        var result = Column.Create(name, order, color);

        // Assert
        Assert.False(result.IsError); 
        Assert.NotNull(result.Value); 
    
        var column = result.Value;
        Assert.Equal(name, column.Name);
        Assert.Equal(order, column.Order);
        Assert.Equal(color, column.Color);
        
        Assert.NotEqual(Guid.Empty, column.Id);
        Assert.NotEqual(default, column.CreatedAt);
        Assert.NotEqual(default, column.UpdatedAt);
    }
    
    [Fact]
    public void Create_ShouldReturnInvalidOrder_WhenOrderIsNegative()
    {
        // Arrange
        const int order = -1;
        
        // Act
        var result = Column.Create("Kanban Column", order);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ColumnErrors.InvalidOrder.Code, result.FirstError.Code);
    }
    
    [Theory]
    [InlineData("red")]    
    [InlineData("#ZZZ")]      
    [InlineData("123456")]    
    public void Create_ShouldReturnError_WhenColorIsInvalid(string invalidColor)
    {
        // Act
        var result = Column.Create("Test", 0, invalidColor);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ColumnErrors.InvalidColorFormat(invalidColor).Code, result.FirstError.Code);
    }
    
    [Fact]
    public void Update_ShouldUpdateFields_WhenValidDataIsProvided()
    {
        // Arrange
        var column = Column.Create("Old Name", 1, "#000000").Value;
        var newName = "New Name";
        var newColor = "#FFFFFF";

        // Act
        var result = column.Update(newName, newColor);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(newName, column.Name);
        Assert.Equal(newColor, column.Color);
    }

    [Fact]
    public void Update_ShouldNotUpdateName_WhenNameIsWhiteSpace()
    {
        // Arrange
        var originalName = "Keep Me";
        var column = Column.Create(originalName, 1, "#000").Value;

        // Act
        column.Update("   ", "#FFF");

        // Assert
        Assert.Equal(originalName, column.Name); 
        Assert.Equal("#FFF", column.Color);      
    }

    [Fact]
    public void Update_ShouldReturnError_WhenColorIsInvalid()
    {
        // Arrange
        var column = Column.Create("Test", 1, "#000").Value;
        var invalidColor = "not-a-color";

        // Act
        var result = column.Update("New Name", invalidColor);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ColumnErrors.InvalidColorFormat(invalidColor).Code, result.FirstError.Code);
    }

    [Fact]
    public void Update_ShouldUpdateTimestamp_WhenCalled()
    {
        // Arrange
        var column = Column.Create("Test", 1, "#000").Value;
        var initialUpdateDate = column.UpdatedAt;
        
        Thread.Sleep(1); 

        // Act
        column.Update("Updated", "#111");

        // Assert
        Assert.True(column.UpdatedAt > initialUpdateDate);
    }
    
    [Fact]
    public void UpdateOrder_ShouldUpdateOrderAndTimestamp_WhenOrderIsValid()
    {
        // Arrange
        var column = Column.Create("Todo", 0).Value;
        var initialUpdateDate = column.UpdatedAt;
        const int newOrder = 5;
        
        Thread.Sleep(1);

        // Act
        var result = column.UpdateOrder(newOrder);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(newOrder, column.Order);
        Assert.True(column.UpdatedAt > initialUpdateDate);
    }
    
    [Fact]
    public void UpdateOrder_ShouldAcceptZero_AsValidOrder()
    {
        // Arrange
        var column = Column.Create("Todo", 10).Value;

        // Act
        var result = column.UpdateOrder(0);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(0, column.Order);
    }
    
    [Fact]
    public void UpdateOrder_ShouldReturnError_WhenOrderIsNegative()
    {
        // Arrange
        var column = Column.Create("Todo", 1).Value;
        const int invalidOrder = -1;

        // Act
        var result = column.UpdateOrder(invalidOrder);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ColumnErrors.InvalidOrder.Code, result.FirstError.Code);
        Assert.Equal(1, column.Order);
    }
    
    [Fact]
    public void AddTask_ShouldInsertAtLastPosition_WhenTaskIsNew()
    {
        // Arrange
        var column = Column.Create("To Do", 0).Value;
        var firstTask = Guid.NewGuid();
        var secondTask = Guid.NewGuid();

        // Act
        column.AddTask(firstTask);
        column.AddTask(secondTask);

        // Assert
        Assert.Equal(2, column.TaskIds.Count);
        Assert.Equal(secondTask, column.TaskIds.Last());
    }

    [Fact]
    public void AddTask_ShouldNotAddDuplicate_WhenTaskIdAlreadyExists()
    {
        // Arrange
        var column = Column.Create("To Do", 0).Value;
        var taskId = Guid.NewGuid();

        // Act
        column.AddTask(taskId);
        column.AddTask(taskId); 

        // Assert
        Assert.Single(column.TaskIds);
    }
    
    [Fact]
    public void RemoveTask_ShouldRemoveIdAndUpdateTimestamp_WhenTaskExists()
    {
        // Arrange
        var column = Column.Create("To Do", 0).Value;
        var taskId = Guid.NewGuid();
        column.AddTask(taskId);
        var dateAfterAdd = column.UpdatedAt;

        Thread.Sleep(1); // Pausa para el timestamp

        // Act
        column.RemoveTask(taskId);

        // Assert
        Assert.Empty(column.TaskIds);
        Assert.True(column.UpdatedAt > dateAfterAdd);
    }

    [Fact]
    public void RemoveTask_ShouldDoNothing_WhenTaskDoesNotExist()
    {
        // Arrange
        var column = Column.Create("To Do", 0).Value;
        var existingTask = Guid.NewGuid();
        var nonExistentTask = Guid.NewGuid();
        column.AddTask(existingTask);
        var lastUpdate = column.UpdatedAt;

        // Act
        column.RemoveTask(nonExistentTask);

        // Assert
        Assert.Single(column.TaskIds);
        Assert.Equal(lastUpdate, column.UpdatedAt);
    }
}