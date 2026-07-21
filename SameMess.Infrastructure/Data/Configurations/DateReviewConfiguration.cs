using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class DateReviewConfiguration : IEntityTypeConfiguration<DateReview>
{
    public void Configure(EntityTypeBuilder<DateReview> builder)
    {
        builder.ToTable("DateReviews", "chat");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(r => r.Comment).HasMaxLength(500);
        builder.Property(r => r.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");

        // Mỗi người chỉ đánh giá 1 lần cho 1 buổi hẹn (1 đơn voucher).
        builder.HasIndex(r => new { r.DatePassOrderId, r.ReviewerId }).IsUnique();
        // Truy vấn nhanh review về 1 người (gắn vào hồ sơ).
        builder.HasIndex(r => r.RevieweeId);
    }
}
