using ErrorOr;
using KanbanAppApi.Common.Events;
using Mediator;

namespace KanbanAppApi.Domain.TaskAggregate;

public enum PriorityType
{
    Low,
    Medium,
    High
}

public record struct TaskCreatedEvent(Guid TaskId, Guid ColumnId) : INotification;
public record struct TaskDeletedEvent(Guid TaskId, Guid ColumnId) : INotification;

public class Task : AggregateRoot
{
    private const int MaxSubTasks = 20;
    
    public Guid Id { get; private init; }
    public string Title { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public bool IsCompleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public PriorityType Priority { get; private  set; }
    public Guid ColumnId { get; private set; }
    
    private readonly List<SubTask> _subTasks  = [];
    public IReadOnlyCollection<SubTask> SubTasks => _subTasks.AsReadOnly();
    
    private Task() { }
    public Task(string title, PriorityType priority, Guid columnId, string? description, bool? isCompleted )
    {
        Id = Guid.NewGuid();
        Title = title;
        Priority = priority;
        ColumnId = columnId;
        Description = description ?? Description;
        IsCompleted = isCompleted ?? IsCompleted;
        
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        RaiseDomainEvent(new TaskCreatedEvent(Id, columnId));
    }

    public ErrorOr<Success> Update(string? title, string? description, bool? isCompleted, PriorityType? priority)
    {
        if (title is not null && title.Trim().Length == 0)
            return TaskErrors.TitleRequired;
        
        if (title?.Length > 250)
            return TaskErrors.TitleTooLong(250);
        
        if (description is not null && description.Length > 1000)
            return TaskErrors.DescriptionTooLong(1000);
        
        Title = title ?? Title;
        Description = description ?? Description;
        IsCompleted = isCompleted ?? IsCompleted;
        Priority = priority ?? Priority;
    
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }
    
    public void Delete() => RaiseDomainEvent(new TaskDeletedEvent(Id, ColumnId));

    public ErrorOr<SubTask> AddSubTask(string description, bool? isCompleted)
    {
        if (_subTasks.Count >= MaxSubTasks)
            return TaskErrors.MaxSubTasksReached(MaxSubTasks);

        var subTask= new SubTask(description, isCompleted);
        _subTasks.Add(subTask);
        
        return subTask;
    }

    public ErrorOr<string> RemoveSubTask(Guid id)
    {
        var subTask = _subTasks.FirstOrDefault(s => s.Id == id);
        if (subTask is null) return TaskErrors.SubTaskNotFound(id.ToString());
        
        _subTasks.Remove(subTask);
        return "Subtask deleted successfully";
    }

    public ErrorOr<SubTask> EditSubTask(Guid id, string? description, bool? isCompleted)
    {
        var subTask = _subTasks.FirstOrDefault(c => c.Id == id);
        if (subTask is null) return TaskErrors.SubTaskNotFound(id.ToString());
        
        var result = subTask.Update(description, isCompleted);
        return result.IsError ? result.Errors : subTask;
    }
}


