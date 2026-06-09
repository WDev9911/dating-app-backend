using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        builder.ToTable("OtpCodes", "auth");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(o => o.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(o => o.Purpose)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(o => new { o.Email, o.Purpose });

        builder.Property(o => o.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Ignore(o => o.IsValid);
    }
}
