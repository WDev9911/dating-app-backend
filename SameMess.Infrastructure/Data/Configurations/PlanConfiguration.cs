using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;

namespace SameMess.Infrastructure.Data.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans", "billing");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.Code).IsUnique();

        // Seed gói (giá để DB, chỉnh không cần build lại). Guid cố định để migration ổn định.
        builder.HasData(
            new Plan
            {
                Id = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                Code = PlanCode.Free, Name = "Free", PriceVnd = 0, DurationDays = 0, IsActive = true,
            },
            new Plan
            {
                Id = Guid.Parse("a0000000-0000-0000-0000-000000000002"),
                Code = PlanCode.Plus, Name = "Plus", PriceVnd = 59000, DurationDays = 30, IsActive = true,
            },
            new Plan
            {
                Id = Guid.Parse("a0000000-0000-0000-0000-000000000003"),
                Code = PlanCode.Gold, Name = "Gold", PriceVnd = 129000, DurationDays = 30, IsActive = true,
            });
    }
}
