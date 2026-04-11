using Bogus;
using ErrorOr;
using KanbanAppApi.Common.Domain.TaskAggregate;
using Task = KanbanAppApi.Common.Domain.TaskAggregate.Task;

namespace KanbanAppApi.UnitTests.Common.Domain.TaskAggregate;

public class TaskTests
{
    private const int MaxSubTasks = 20;
    private readonly Faker _faker = new();
  
    private Task CreateInitialTask()
    {
         return Task.Create(
          _faker.Commerce.ProductName(),
          PriorityType.Medium,
          Guid.NewGuid(),
          _faker.Lorem.Sentence()
        ).Value;
    }
    
    private Task CreateTaskWithSubTasks(int count)
    {
        var task = Task.Create(
            _faker.Commerce.ProductName(),
            PriorityType.Medium,
            Guid.NewGuid()
        ).Value;

        for (int i = 0; i < count; i++)
        {
            task.AddSubTask(_faker.Lorem.Sentence(), false);
        }

        return task;
    }

    [Fact]
    public void Create_ShouldReturnTask_WhenDataIsValid()
    {
        var title = _faker.Commerce.ProductName();
        var priority = _faker.PickRandom<PriorityType>();
        var columnId = Guid.NewGuid();
        var description = _faker.Lorem.Sentence();

        // Act
        var result = Task.Create(title, priority, columnId, description);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(title, result.Value.Title);
        Assert.Equal(priority, result.Value.Priority);
        Assert.Equal(description, result.Value.Description);
    }
    
    [Fact]
    public void Create_ShouldRaiseTaskCreatedEvent()
    {
        // Arrange
        var title = _faker.Commerce.ProductName();
        var columnId = Guid.NewGuid();

        // Act
        var result = Task.Create(title, PriorityType.Medium, columnId);

        // Assert
        Assert.False(result.IsError);
        var task = result.Value;

  
        var createdEvent = Assert.Single(task.DomainEvents);
        var taskCreated = Assert.IsType<TaskCreatedEvent>(createdEvent);

        Assert.Equal(task.Id, taskCreated.TaskId);
        Assert.Equal(columnId, taskCreated.ColumnId);
    }

    [Fact]
    public void Create_ShouldReturnMultipleErrors_WhenRequiredFieldsAreInvalid()
    {
        // Arrange
        var invalidTitle = ""; 
        var invalidColumnId = Guid.Empty; 
        var priority = _faker.PickRandom<PriorityType>();

        // Act
        var result = Task.Create(invalidTitle, priority, invalidColumnId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains(result.Errors, e => e.Code == TaskErrors.TitleRequired.Code);
        Assert.Contains(result.Errors, e => e.Code == TaskErrors.InvalidColumnId.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenDescriptionIsTooLong()
    {
        // Arrange
        var title = _faker.Commerce.ProductName();
        var columnId = Guid.NewGuid();
        var longDescription = _faker.Lorem.Letter(1001); 

        // Act
        var result = Task.Create(title, PriorityType.Medium, columnId, longDescription);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(TaskErrors.DescriptionTooLong(1000).Code, result.FirstError.Code);
    }

    [Fact]
    public void Create_ShouldHandleOptionalFieldsCorrectly()
    {
        // Arrange
        var title = _faker.Commerce.ProductName();
        var columnId = Guid.NewGuid();
        var isCompleted = _faker.Random.Bool();

        // Act
        var result = Task.Create(title, PriorityType.Low, columnId, description: null, isCompleted: isCompleted);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(result.Value.Description, string.Empty);
        Assert.Equal(isCompleted, result.Value.IsCompleted);
    }
    
    [Fact]
    public void Create_ShouldAssignCreatedAt_WhenTaskIsCreated()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;
        var title = _faker.Commerce.ProductName();
        var columnId = Guid.NewGuid();

        // Act
        var result = Task.Create(title, PriorityType.Medium, columnId);

        // Assert
        Assert.False(result.IsError);
    
        // default (01/01/0001)
        Assert.NotEqual(default, result.Value.CreatedAt);
        Assert.True(result.Value.CreatedAt >= beforeCreation);
        Assert.Equal(DateTimeKind.Utc, result.Value.CreatedAt.Kind);
    }
    
    [Fact]
    public void Create_ShouldAssignSameValueToCreatedAtAndUpdatedAt()
    {
        // Arrange
        var title = _faker.Commerce.ProductName();
        var columnId = Guid.NewGuid();

        // Act
        var result = Task.Create(title, PriorityType.Medium, columnId);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(result.Value.CreatedAt, result.Value.UpdatedAt);
    }
    
    [Fact]
    public void Update_ShouldSucceed_WhenAllDataIsValid()
    {
        // Arrange
        var task = CreateInitialTask();
        var newTitle = _faker.Commerce.ProductName();
        var newDescription = _faker.Lorem.Sentence();
        var newPriority = PriorityType.High;
        var newStatus = true;

        // Act
        var result = task.Update(newTitle, newDescription, newStatus, newPriority);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(newTitle, task.Title);
        Assert.Equal(newDescription, task.Description);
        Assert.Equal(newPriority, task.Priority);
        Assert.True(task.IsCompleted);
    }
    
    [Fact]
    public void Update_ShouldKeepOriginalValues_WhenArgumentsAreNull()
    {
        // Arrange
        var task = CreateInitialTask();
        var originalTitle = task.Title;
        var originalDescription = task.Description;

        // Act
        var result = task.Update(null, null, null, null);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(originalTitle, task.Title);
        Assert.Equal(originalDescription, task.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_ShouldReturnTitleRequired_WhenTitleIsProvidedButEmpty(string invalidTitle)
    {
        // Arrange
        var task = CreateInitialTask();

        // Act
        var result = task.Update(invalidTitle, null, null, null);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(TaskErrors.TitleRequired.Code, result.FirstError.Code);
    }

    [Fact]
    public void Update_ShouldReturnTitleTooLong_WhenTitleExceedsLimit()
    {
        // Arrange
        var task = CreateInitialTask();
        var longTitle = _faker.Lorem.Letter(251);

        // Act
        var result = task.Update(longTitle, null, null, null);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(TaskErrors.TitleTooLong(250).Code, result.FirstError.Code);
    }

    [Fact]
    public void Update_ShouldReturnDescriptionTooLong_WhenDescriptionExceedsLimit()
    {
        // Arrange
        var task = CreateInitialTask();
        var longDescription = _faker.Lorem.Letter(1001);

        // Act
        var result = task.Update(null, longDescription, null, null);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(TaskErrors.DescriptionTooLong(1000).Code, result.FirstError.Code);
    }

    [Fact]
    public void Update_ShouldUpdateTimestamp_WhenSuccessful()
    {
        // Arrange
        var task = CreateInitialTask();
        var initialUpdatedAt = task.UpdatedAt;
        
        Thread.Sleep(1); 

        // Act
        task.Update(_faker.Commerce.ProductName(), null, null, null);

        // Assert
        Assert.True(task.UpdatedAt > initialUpdatedAt);
    }
    
    [Fact]
    public void AddSubTask_ShouldSucceed_AndAddToCollection()
    {
        // Arrange
        var task = CreateInitialTask();
        var description = _faker.Lorem.Sentence();
        var isCompleted = _faker.Random.Bool();

        // Act
        var result = task.AddSubTask(description, isCompleted);

        // Assert
        Assert.False(result.IsError);
        Assert.Single(task.SubTasks); 
        Assert.Equal(description, result.Value.Description);
        Assert.Equal(isCompleted, result.Value.IsCompleted);
    }

    [Fact]
    public void AddSubTask_ShouldReturnError_WhenMaxLimitIsReached()
    {
        // Arrange
        var task = CreateInitialTask();
        
        for (int i = 0; i < MaxSubTasks; i++)
        {
            task.AddSubTask(_faker.Lorem.Word(), false);
        }
        
        var result = task.AddSubTask("One too many", false);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(TaskErrors.MaxSubTasksReached(MaxSubTasks).Code, result.FirstError.Code);
        Assert.Equal(MaxSubTasks, task.SubTasks.Count); 
    }

    [Fact]
    public void AddSubTask_ShouldHandleNullCompleted_ByUsingDefault()
    {
        // Arrange
        var task = CreateInitialTask();

        // Act
        var result = task.AddSubTask(_faker.Lorem.Sentence(), null);

        // Assert
        Assert.False(result.IsError);
        Assert.False(result.Value.IsCompleted);
    }
    
    [Fact]
    public void AddSubTask_ShouldUpdateTimestamp_WhenSuccessful()
    {
        // Arrange
        var task = CreateInitialTask();
        var initialUpdatedAt = task.UpdatedAt;

        Thread.Sleep(1);

        // Act
        var result = task.AddSubTask(_faker.Lorem.Sentence(), false);

        // Assert
        Assert.False(result.IsError);
        Assert.True(task.UpdatedAt > initialUpdatedAt);
    }
    
    [Fact]
    public void RemoveSubTask_ShouldReturnDeleted_WhenSubTaskExists()
    {
        // Arrange
        var task = CreateTaskWithSubTasks(3);
        var subTaskToRemove = task.SubTasks.First();
        var initialCount = task.SubTasks.Count;

        // Act
        var result = task.RemoveSubTask(subTaskToRemove.Id);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Result.Deleted, result.Value);
        Assert.Equal(initialCount - 1, task.SubTasks.Count);
        Assert.DoesNotContain(task.SubTasks, s => s.Id == subTaskToRemove.Id);
    }

    [Fact]
    public void RemoveSubTask_ShouldReturnNotFound_WhenSubTaskDoesNotExist()
    {
        // Arrange
        var task = CreateTaskWithSubTasks(2);
        var randomId = Guid.NewGuid();

        // Act
        var result = task.RemoveSubTask(randomId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
        Assert.Equal(TaskErrors.SubTaskNotFound(randomId.ToString()).Code, result.FirstError.Code);
        Assert.Equal(2, task.SubTasks.Count); // 
    }

    [Fact]
    public void RemoveSubTask_ShouldHandleEmptyList()
    {
        // Arrange
        var task = CreateTaskWithSubTasks(0);
        var randomId = Guid.NewGuid();

        // Act
        var result = task.RemoveSubTask(randomId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(TaskErrors.SubTaskNotFound(randomId.ToString()).Code, result.FirstError.Code);
    }

    [Fact]
    public void RemoveSubTask_ShouldUpdateTimestamp_WhenSuccessful()
    {
        // Arrange
        var task = CreateTaskWithSubTasks(2);
        var subTaskToRemove = task.SubTasks.First();
        var initialUpdatedAt = task.UpdatedAt;
        
        Thread.Sleep(1);
        
        // Act
        var result = task.RemoveSubTask(subTaskToRemove.Id);
        
        // Assert
        Assert.False(result.IsError);
        Assert.True(task.UpdatedAt > initialUpdatedAt);
    }
    
    [Fact]
    public void EditSubTask_ShouldSucceed_AndUpdateParentTimestamp()
    {
        // Arrange
        var task = CreateTaskWithSubTasks(2);
        var newDescription = _faker.Lorem.Sentence();
        var newStatus = true;
        var subTaskToEdit = task.SubTasks.First();
        var initialUpdatedAt = task.UpdatedAt;

        Thread.Sleep(1);

        // Act
        var result = task.EditSubTask(subTaskToEdit.Id, newDescription, newStatus);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(newDescription, subTaskToEdit.Description);
        Assert.Equal(newStatus, subTaskToEdit.IsCompleted);
        Assert.True(task.UpdatedAt > initialUpdatedAt);
    }

    [Fact]
    public void EditSubTask_ShouldReturnNotFound_WhenIdDoesNotExist()
    {
        // Arrange
        var task = CreateTaskWithSubTasks(3);
        var randomId = Guid.NewGuid();
    
        // Act
        var result = task.EditSubTask(randomId, "New Desc", true);
    
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
        Assert.Equal(TaskErrors.SubTaskNotFound(randomId.ToString()).Code, result.FirstError.Code);
    }
    
    [Fact]
    public void Delete_ShouldRaiseTaskDeletedEvent()
    {
        // Arrange
        var task = Task.Create(_faker.Commerce.ProductName(), PriorityType.Medium, Guid.NewGuid()).Value;

        // Act
        task.Delete();

        // Assert
    
        Assert.Contains(task.DomainEvents, e => e is TaskDeletedEvent);
    
        var deletedEvent = task.DomainEvents.OfType<TaskDeletedEvent>().Single();
        Assert.Equal(task.Id, deletedEvent.TaskId);
    }
    
}