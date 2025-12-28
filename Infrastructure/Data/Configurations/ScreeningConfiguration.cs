using Cinema_MGMT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema_MGMT.Infrastructure.Data.Configurations;

public class ScreeningConfiguration : IEntityTypeConfiguration<Screening>
{
    public void Configure(EntityTypeBuilder<Screening> builder)
    {
        builder.ToTable("Screenings");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.MovieId)
            .IsRequired();

        builder.Property(s => s.AuditoriumId)
            .IsRequired();

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.Property(s => s.EndTime)
            .IsRequired();

        builder.Property(s => s.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasMany(s => s.Bookings)
            .WithOne(b => b.Screening)
            .HasForeignKey(b => b.ScreeningId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index for performance
        builder.HasIndex(s => s.StartTime);
        builder.HasIndex(s => new { s.AuditoriumId, s.StartTime });
    }
}

