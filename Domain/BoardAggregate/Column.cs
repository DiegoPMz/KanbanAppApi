namespace KanbanAppApi.Domain.BoardAggregate;

public class Column
{
    public Guid Id { get;  private init; }
    public string Name { get; private set; }
    public int Order { get;  private set; }
    public string Color { get;  private set; }
    public DateTime CreatedAt { get; private init; }
    public DateTime UpdatedAt { get; private set; }
    
    private readonly List<Guid> _taskIds = [];
    public IReadOnlyCollection<Guid> TaskIds => _taskIds.AsReadOnly();
    
    private Column() { }

    public Column (string name, int order, string? color)
    {
        Name = name;
        Order = order;
        Color = color ?? "#0387";
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string? name, string? color)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name;
        if (!string.IsNullOrWhiteSpace(color)) Color = color;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateOrder(int newOrder)
    {
        Order = newOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTask(Guid taskId)
    {
        _taskIds.Insert(0, taskId);
    }

    public void RemoveTask(Guid taskId)
    {
        if (_taskIds.Contains(taskId))
        {
            _taskIds.Remove(taskId);
            UpdatedAt = DateTime.UtcNow; 
        }
    }
}