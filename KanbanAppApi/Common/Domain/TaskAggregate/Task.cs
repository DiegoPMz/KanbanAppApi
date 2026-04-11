using ErrorOr;
using KanbanAppApi.Common.Events;
using Mediator;

namespace KanbanAppApi.Common.Domain.TaskAggregate;

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
    private const int MaxTitleLength = 250; 
    private const int MaxDescriptionLength = 1000; 
    
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
    private Task(string title, PriorityType priority, Guid columnId, string? description, bool? isCompleted)
    {
        Id = Guid.NewGuid();
        Title = title;
        Priority = priority;
        ColumnId = columnId;
        Description = description ?? Description;
        IsCompleted = isCompleted ?? false;
        
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        
        RaiseDomainEvent(new TaskCreatedEvent(Id, columnId));
    }
    
    public static ErrorOr<Task> Create(string title, PriorityType priority, Guid columnId, string? description = null, bool? isCompleted = null )
    {
        List<Error> errors = [];
        
        if (string.IsNullOrWhiteSpace(title)) 
            errors.Add(TaskErrors.TitleRequired);
        
        if (title.Length > MaxTitleLength) 
            errors.Add(TaskErrors.TitleTooLong(MaxTitleLength));

        if (!Enum.IsDefined(typeof(PriorityType), priority))
            errors.Add(TaskErrors.InvalidPriority);

        if (columnId == Guid.Empty)
            errors.Add(TaskErrors.InvalidColumnId);

        if (description is not null)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                errors.Add(TaskErrors.InvalidDescription);
            }
            else if (description.Length > MaxDescriptionLength)
            {
                errors.Add(TaskErrors.DescriptionTooLong(MaxDescriptionLength));
            }
        }

        if (errors.Count > 0) return errors;
        
        return new Task(title, priority, columnId, description, isCompleted);
    }
    
    public ErrorOr<Updated> Update(string? title, string? description, bool? isCompleted, PriorityType? priority)
    {
        if (title is not null && title.Trim().Length == 0)
            return TaskErrors.TitleRequired;
        
        if (title?.Length > MaxTitleLength)
            return TaskErrors.TitleTooLong(MaxTitleLength);
        
        if (description is not null && description.Length > MaxDescriptionLength)
            return TaskErrors.DescriptionTooLong(MaxDescriptionLength);
        
        Title = title ?? Title;
        Description = description ?? Description;
        IsCompleted = isCompleted ?? IsCompleted;
        Priority = priority ?? Priority;
    
        UpdatedAt = DateTime.UtcNow;
        return Result.Updated;
    }
    
    public void Delete() => RaiseDomainEvent(new TaskDeletedEvent(Id, ColumnId));

    public ErrorOr<SubTask> AddSubTask(string description, bool? isCompleted = null)
    {
        if (_subTasks.Count >= MaxSubTasks) 
            return TaskErrors.MaxSubTasksReached(MaxSubTasks);

        var subTask= SubTask.Create(description, isCompleted);
        if (subTask.IsError) return subTask.Errors;
        
        _subTasks.Add(subTask.Value);
        
        UpdatedAt = DateTime.UtcNow;
        return subTask;
    }

    public ErrorOr<Deleted> RemoveSubTask(Guid id)
    {
        var subTask = _subTasks.FirstOrDefault(s => s.Id == id);
        if (subTask is null) return TaskErrors.SubTaskNotFound(id.ToString());
        
        _subTasks.Remove(subTask);
        
        UpdatedAt = DateTime.UtcNow;
        return Result.Deleted;
    }

    public ErrorOr<SubTask> EditSubTask(Guid id, string? description, bool? isCompleted)
    {
        var subTask = _subTasks.FirstOrDefault(c => c.Id == id);
        
        if (subTask is null) 
            return TaskErrors.SubTaskNotFound(id.ToString());
        
        var result = subTask.Update(description, isCompleted);

        if (result.IsError) return result.Errors;
        
        UpdatedAt = DateTime.UtcNow;
        return subTask;
    }
}


