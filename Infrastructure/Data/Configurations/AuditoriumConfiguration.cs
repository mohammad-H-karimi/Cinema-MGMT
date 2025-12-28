using Cinema_MGMT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema_MGMT.Infrastructure.Data.Configurations;

public class AuditoriumConfiguration : IEntityTypeConfiguration<Auditorium>
{
    public void Configure(EntityTypeBuilder<Auditorium> builder)
    {
        builder.ToTable("Auditoriums");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Capacity)
            .IsRequired();

        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasMany(a => a.Screenings)
            .WithOne(s => s.Auditorium)
            .HasForeignKey(s => s.AuditoriumId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Seats)
            .WithOne(s => s.Auditorium)
            .HasForeignKey(s => s.AuditoriumId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

