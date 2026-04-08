using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KanbanAppApi.Features.Auth.Shared;

public class TokenDbConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToTable("tokens");
        builder.HasKey(e => e.SessionId);

        builder.Property(e => e.Type)
            .HasMaxLength(20) 
            .HasConversion<string>()
            .IsRequired();
            
        builder.Property(e => e.Provider)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.ExpiresAt)
            .IsRequired();

        builder.HasOne<Domain.User.User>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.UserId);
    }
}