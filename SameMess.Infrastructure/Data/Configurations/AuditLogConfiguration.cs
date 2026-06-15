using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "admin");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(a => a.Action).IsRequired().HasMaxLength(100);
        builder.Property(a => a.TargetType).HasMaxLength(50);
        builder.Property(a => a.Details).HasMaxLength(2000);

        builder.HasIndex(a => a.CreatedAt);
        builder.HasIndex(a => a.Action);
        builder.HasIndex(a => a.AdminId);

        builder.Property(a => a.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
    }
}
