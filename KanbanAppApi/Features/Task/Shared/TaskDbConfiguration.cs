using KanbanAppApi.Common.Domain.BoardAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TaskModel = KanbanAppApi.Common.Domain.TaskAggregate.Task;

namespace KanbanAppApi.Features.Task.Shared;

public class TaskDbConfiguration: IEntityTypeConfiguration<TaskModel>
{
    public void Configure(EntityTypeBuilder<TaskModel> builder)
    {
            builder.ToTable("tasks");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedNever();
            
            builder.Property(e => e.Title)
                .HasMaxLength(250)
                .IsRequired();
            
            builder.Property(e => e.Description) 
                .HasMaxLength(1000)
                .HasDefaultValue(string.Empty);

            builder.Property(e => e.Priority)
                .HasConversion<string>();

            builder.Property(e => e.IsCompleted)
                .HasDefaultValue(false);

            builder.HasOne<Column>() 
                .WithMany() 
                .HasForeignKey(t => t.ColumnId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(b => b.SubTasks)
                .WithOne()
                .HasForeignKey("task_id")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
    }
}