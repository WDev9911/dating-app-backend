using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
{
    public void Configure(EntityTypeBuilder<FeatureFlag> builder)
    {
        builder.ToTable("FeatureFlags", "admin");
        builder.HasKey(f => f.Key);
        builder.Property(f => f.Key).HasMaxLength(100);
        builder.Property(f => f.IsEnabled).HasDefaultValue(false);
        builder.Property(f => f.UpdatedAt).HasDefaultValueSql("now() at time zone 'utc'");
    }
}
