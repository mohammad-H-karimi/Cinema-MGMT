using Cinema_MGMT.Domain.Entities;
using Cinema_MGMT.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema_MGMT.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.BookingId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Method)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.PaymentDate)
            .IsRequired();

        builder.Property(p => p.TransactionId)
            .HasMaxLength(200);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(p => p.BookingId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.TransactionId);
    }
}

