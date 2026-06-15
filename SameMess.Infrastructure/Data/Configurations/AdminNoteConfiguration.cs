using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class AdminNoteConfiguration : IEntityTypeConfiguration<AdminNote>
{
    public void Configure(EntityTypeBuilder<AdminNote> builder)
    {
        builder.ToTable("AdminNotes", "admin");

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(n => n.Content).IsRequired().HasMaxLength(2000);
        builder.HasIndex(n => n.UserId);
        builder.Property(n => n.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
