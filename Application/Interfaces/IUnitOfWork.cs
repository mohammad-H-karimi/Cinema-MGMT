using Cinema_MGMT.Domain.Entities;

namespace Cinema_MGMT.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Movie> Movies { get; }
    IRepository<Screening> Screenings { get; }
    IRepository<Auditorium> Auditoriums { get; }
    IRepository<Seat> Seats { get; }
    IRepository<Booking> Bookings { get; }
    IRepository<BookingSeat> BookingSeats { get; }
    IRepository<Payment> Payments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

