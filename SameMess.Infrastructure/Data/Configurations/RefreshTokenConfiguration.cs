using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", "auth");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(r => r.TokenHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(r => r.TokenHash);

        builder.Property(r => r.CreatedByIp)
            .HasMaxLength(50);

        builder.Property(r => r.UserAgent)
            .HasMaxLength(500);

        builder.Property(r => r.ReplacedByTokenHash)
            .HasMaxLength(500);

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Ignore(r => r.IsActive);
    }
}
