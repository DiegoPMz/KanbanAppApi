using FluentResults;

namespace KanbanAppApi.Core.Errors;

public enum SeverityValues
{
    Info, 
    Warning, 
    Critical
}

public class AppError : Error
{
    public string Code { get; private set; }
    public SeverityValues Severity { get; private set; }
    public string Source { get; private set; }
    public Guid CorrelationId { get; private set; }
    public bool Retryable { get; private set; }
    public bool UserFacing { get; private set; }
    public DateTime Date { get; private set; }

    public AppError(string message) : base(message)
    {
        Date = DateTime.UtcNow;
    }

    // Builder fluido
    public AppError WithCode(string code)
    {
        Code = code;
        return this;
    }

    public AppError WithSeverity(SeverityValues severity)
    {
        Severity = severity;
        return this;
    }

    public AppError WithSource(string source)
    {
        Source = source;
        return this;
    }

    public AppError WithCorrelationId(Guid correlationId)
    {
        CorrelationId = correlationId;
        return this;
    }

    public AppError WithRetryable(bool retryable)
    {
        Retryable = retryable;
        return this;
    }

    public AppError WithUserFacing(bool userFacing)
    {
        UserFacing = userFacing;
        return this;
    }
}
