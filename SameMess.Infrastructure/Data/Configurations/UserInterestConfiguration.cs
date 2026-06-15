using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class UserInterestConfiguration : IEntityTypeConfiguration<UserInterest>
{
    public void Configure(EntityTypeBuilder<UserInterest> builder)
    {
        builder.ToTable("UserInterests", "auth");

        builder.HasKey(ui => ui.Id);

        builder.Property(ui => ui.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.HasIndex(ui => new { ui.UserId, ui.InterestId }).IsUnique();

        builder.HasOne(ui => ui.User)
            .WithMany()
            .HasForeignKey(ui => ui.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ui => ui.Interest)
            .WithMany(i => i.UserInterests)
            .HasForeignKey(ui => ui.InterestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
