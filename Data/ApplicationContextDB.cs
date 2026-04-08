using KanbanAppApi.Common.Events;
using KanbanAppApi.Domain.BoardAggregate;
using KanbanAppApi.Domain.TaskAggregate;
using KanbanAppApi.Domain.User;
using KanbanAppApi.Features.Auth.Shared;
using Mediator;
using Microsoft.EntityFrameworkCore;
using TaskModel = KanbanAppApi.Domain.TaskAggregate.Task;

namespace KanbanAppApi.Data;

public class ApplicationContextDb : DbContext
{
    private readonly IMediator _mediator;
    
    public ApplicationContextDb(DbContextOptions<ApplicationContextDb> options, IMediator mediator) 
        : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<Board> Boards { get; set; }
    public DbSet<Column> Columns { get; set; }
    public DbSet<TaskModel> Tasks { get; set; }
    public DbSet<SubTask> SubTasks { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContextDb).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count != 0)
            .SelectMany(e => 
            {
                var events = e.DomainEvents.ToList();
                e.ClearDomainEvents(); 
                return events;
            })
            .ToList();
        
        var result = await base.SaveChangesAsync(ct);
        
        if (domainEvents.Count != 0) 
        {
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, ct);
            }
        }

        return result;
    }
}