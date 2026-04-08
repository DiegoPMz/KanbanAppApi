using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KanbanAppApi.Features.User.Shared;

public class UserDbConfiguration : IEntityTypeConfiguration<Domain.User.User>
{
    public void Configure(EntityTypeBuilder<Domain.User.User> builder)
    {
        builder.ToTable("users");
            
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.HasIndex(e => e.ExternalId).IsUnique();
        builder.Property(e => e.ExternalId)
            .HasMaxLength(400)
            .IsRequired();

        builder.HasIndex(e => e.Email).IsUnique();
        builder.Property(e => e.Email)
            .HasMaxLength(200)
            .IsRequired();
            
        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired(); 

        builder.Property(e => e.FamilyName)
            .HasMaxLength(200)
            .IsRequired(false); 

        builder.Property(e => e.PictureUrl)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(e => e.AppTheme)
            .HasMaxLength(50);
    }
}