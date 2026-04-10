using System.Text.RegularExpressions;
using ErrorOr;

namespace KanbanAppApi.Common.Domain.BoardAggregate;

public partial class Column
{
    public Guid Id { get;  private init; }
    public string Name { get; private set; }
    public int Order { get;  private set; }
    public string Color { get;  private set; }
    public DateTime CreatedAt { get; private init; }
    public DateTime UpdatedAt { get; private set; }
    private readonly List<Guid> _taskIds = [];
    public IReadOnlyCollection<Guid> TaskIds => _taskIds.AsReadOnly();
    
    [GeneratedRegex(@"^#([A-Fa-f0-9]{3}|[A-Fa-f0-9]{4}|[A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$")]
    private static partial Regex ColorRegex();
    
    private Column() { }

    private Column (string name, int order, string? color)
    {
        Id = Guid.NewGuid();
        Name = name;
        Order = order;
        Color = color ?? "#0387";
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public static ErrorOr<Column> Create(string name, int order, string? color = null)
    {
        if (string.IsNullOrWhiteSpace(name)) 
            return ColumnErrors.InvalidName(name);
    
        if (order < 0) 
            return ColumnErrors.InvalidOrder; 
        
        if (!string.IsNullOrEmpty(color) && !ColorRegex().IsMatch(color))
        {
            return ColumnErrors.InvalidColorFormat(color);
        }
    
        return new Column(name, order, color);
    }

    public ErrorOr<Updated> Update(string? name, string? color)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }

        if (!string.IsNullOrWhiteSpace(color))
        {
            if (!ColorRegex().IsMatch(color))
                return ColumnErrors.InvalidColorFormat(color);
            
            Color = color;
        }

        UpdatedAt = DateTime.UtcNow;

        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateOrder(int newOrder)
    {
        if (newOrder < 0) 
            return ColumnErrors.InvalidOrder; 
        
        Order = newOrder;
        UpdatedAt = DateTime.UtcNow;
        
        return Result.Updated;
    }

    public void AddTask(Guid taskId)
    {
        if (_taskIds.Contains(taskId)) return;
        
        _taskIds.Add(taskId);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveTask(Guid taskId)
    {
        if (!_taskIds.Contains(taskId)) return;
        
        _taskIds.Remove(taskId);
        UpdatedAt = DateTime.UtcNow; 
    }
}