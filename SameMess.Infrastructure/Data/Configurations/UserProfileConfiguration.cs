using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles", "auth");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(p => p.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Gender)
            .HasMaxLength(20);

        builder.Property(p => p.DateOfBirth)
            .HasColumnType("date");

        builder.Property(p => p.IsProfileCompleted)
            .HasDefaultValue(false);

        builder.Property(p => p.Bio)
            .HasMaxLength(1000);

        builder.Property(p => p.Location)
            .HasMaxLength(200);

        builder.Property(p => p.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(p => p.DatingGoal)
            .HasMaxLength(100);

        builder.Property(p => p.IsPhotoVerified)
            .HasDefaultValue(false);

        builder.Property(p => p.VerificationStatus)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("None");

        builder.Property(p => p.VerificationSelfieUrl)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Composite index hỗ trợ lọc thô theo bounding box ở Discovery (Phase 2)
        builder.HasIndex(p => new { p.Latitude, p.Longitude });
    }
}
