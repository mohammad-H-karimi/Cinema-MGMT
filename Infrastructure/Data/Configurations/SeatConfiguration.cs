using Cinema_MGMT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema_MGMT.Infrastructure.Data.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("Seats");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.AuditoriumId)
            .IsRequired();

        builder.Property(s => s.Row)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(s => s.Number)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasMany(s => s.BookingSeats)
            .WithOne(bs => bs.Seat)
            .HasForeignKey(bs => bs.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint: Each seat in an auditorium should be unique
        builder.HasIndex(s => new { s.AuditoriumId, s.Row, s.Number })
            .IsUnique();
    }
}

