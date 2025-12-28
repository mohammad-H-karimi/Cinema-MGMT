using Cinema_MGMT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema_MGMT.Infrastructure.Data.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("Movies");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Description)
            .HasMaxLength(2000);

        builder.Property(m => m.DurationMinutes)
            .IsRequired();

        builder.Property(m => m.Genre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Director)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.ReleaseDate)
            .IsRequired();

        builder.Property(m => m.PosterUrl)
            .HasMaxLength(500);

        builder.Property(m => m.TicketPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(m => m.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasMany(m => m.Screenings)
            .WithOne(s => s.Movie)
            .HasForeignKey(s => s.MovieId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

