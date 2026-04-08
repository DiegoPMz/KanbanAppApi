using ErrorOr;

namespace KanbanAppApi.Common.Domain.TaskAggregate;

public class SubTask
{
    private const int MaxLengthDescription = 250;
    
    public Guid Id { get; private init; }
    public string Description { get;  private set; } = string.Empty;
    public bool IsCompleted { get; private set; }
    public DateTime CreatedAt { get; private init; }
    public DateTime UpdatedAt { get; private set; }
    
    private SubTask() { }
    public SubTask(string description, bool? isCompleted)
    {
        Description = description;
        IsCompleted = isCompleted ?? false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public ErrorOr<Success> Update(string? description, bool? isCompleted)
    {
        if (description == Description && isCompleted == IsCompleted)
            return Result.Success;

        var hasChanged = false;

        if (description is not null)
        {
            if (string.IsNullOrWhiteSpace(description))
                return SubTaskErrors.DescriptionRequired;

            if (description.Length > MaxLengthDescription)
                return SubTaskErrors.DescriptionTooLong(MaxLengthDescription);
                
            if (Description != description)
            {
                Description = description;
                hasChanged = true;
            }
        }

        if (isCompleted.HasValue && isCompleted.Value != IsCompleted)
        {
            IsCompleted = isCompleted.Value;
            hasChanged = true;
        }

        if (hasChanged)
        {
            UpdatedAt = DateTime.UtcNow;
        }

        return Result.Success;;
    }
}