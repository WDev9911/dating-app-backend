using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Reports", "safety");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(r => r.Reason)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(r => r.ReportedId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.ReportedId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
