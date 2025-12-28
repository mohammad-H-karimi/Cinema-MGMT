using Cinema_MGMT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema_MGMT.Infrastructure.Data.Configurations;

public class BookingSeatConfiguration : IEntityTypeConfiguration<BookingSeat>
{
    public void Configure(EntityTypeBuilder<BookingSeat> builder)
    {
        builder.ToTable("BookingSeats");

        builder.HasKey(bs => bs.Id);

        builder.Property(bs => bs.BookingId)
            .IsRequired();

        builder.Property(bs => bs.SeatId)
            .IsRequired();

        builder.Property(bs => bs.CreatedAt)
            .IsRequired();

        // Unique constraint: Prevent double booking of the same seat in the same booking
        builder.HasIndex(bs => new { bs.BookingId, bs.SeatId })
            .IsUnique();
    }
}

