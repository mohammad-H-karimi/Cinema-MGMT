using Cinema_MGMT.Domain.Entities;
using Cinema_MGMT.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema_MGMT.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.ScreeningId)
            .IsRequired();

        builder.Property(b => b.UserId)
            .IsRequired();

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(b => b.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(b => b.BookingDate)
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasMany(b => b.BookingSeats)
            .WithOne(bs => bs.Booking)
            .HasForeignKey(bs => bs.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Payment)
            .WithOne(p => p.Booking)
            .HasForeignKey<Payment>(p => p.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.ScreeningId);
        builder.HasIndex(b => b.Status);
    }
}

