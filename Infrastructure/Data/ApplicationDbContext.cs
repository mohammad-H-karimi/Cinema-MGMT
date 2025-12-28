using Cinema_MGMT.Domain.Common;
using Cinema_MGMT.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cinema_MGMT.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Screening> Screenings { get; set; }
    public DbSet<Auditorium> Auditoriums { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingSeat> BookingSeats { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure global query filters if needed
        // Example: modelBuilder.Entity<Movie>().HasQueryFilter(m => m.IsActive);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically set UpdatedAt on entity updates
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && 
                       (e.State == EntityState.Modified || e.State == EntityState.Added));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

