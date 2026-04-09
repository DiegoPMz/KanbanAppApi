using KanbanAppApi.Common.Domain.TaskAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KanbanAppApi.Features.Task.Shared;

public class SubTaskDbConfiguration: IEntityTypeConfiguration<SubTask>
{
    public void Configure(EntityTypeBuilder<SubTask> builder)
    {
        builder.ToTable("subtasks");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Description)
                .HasMaxLength(250)
                .HasDefaultValue(string.Empty);
            
        builder.Property(e => e.IsCompleted)
                .HasDefaultValue(false);
    }
}