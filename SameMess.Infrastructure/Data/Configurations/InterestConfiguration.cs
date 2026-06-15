using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class InterestConfiguration : IEntityTypeConfiguration<Interest>
{
    public void Configure(EntityTypeBuilder<Interest> builder)
    {
        builder.ToTable("Interests", "auth");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(i => i.Name).IsUnique();

        builder.Property(i => i.GroupName)
            .HasMaxLength(50);

        builder.Property(i => i.IsActive)
            .HasDefaultValue(true);

        builder.Property(i => i.CreatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");

        // Seed danh mục sở thích (GUID cố định để migration ổn định, CreatedAt cố định tránh churn)
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var catalog = new (int N, string Name, string Group)[]
        {
            (1, "Du lịch", "Lối sống"), (2, "Ẩm thực", "Lối sống"), (3, "Cà phê", "Lối sống"),
            (4, "Nấu ăn", "Lối sống"), (5, "Thú cưng", "Lối sống"),
            (6, "Phim ảnh", "Giải trí"), (7, "Âm nhạc", "Giải trí"), (8, "Game", "Giải trí"),
            (9, "Đọc sách", "Giải trí"), (10, "Nhiếp ảnh", "Giải trí"),
            (11, "Gym", "Thể thao"), (12, "Yoga", "Thể thao"), (13, "Chạy bộ", "Thể thao"),
            (14, "Bơi lội", "Thể thao"), (15, "Leo núi", "Thể thao"),
            (16, "Vẽ", "Sáng tạo"), (17, "Thời trang", "Sáng tạo"), (18, "Khiêu vũ", "Sáng tạo"),
            (19, "Công nghệ", "Sáng tạo"), (20, "Cắm trại", "Lối sống"),
        };

        builder.HasData(catalog.Select(c => new Interest
        {
            Id = Guid.Parse($"b0000000-0000-0000-0000-{c.N:D12}"),
            Name = c.Name,
            GroupName = c.Group,
            IsActive = true,
            CreatedAt = seedDate,
        }));
    }
}
