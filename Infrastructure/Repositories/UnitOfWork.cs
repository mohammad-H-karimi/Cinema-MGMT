using Cinema_MGMT.Application.Interfaces;
using Cinema_MGMT.Domain.Entities;
using Cinema_MGMT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cinema_MGMT.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IRepository<Movie>? _movies;
    private IRepository<Screening>? _screenings;
    private IRepository<Auditorium>? _auditoriums;
    private IRepository<Seat>? _seats;
    private IRepository<Booking>? _bookings;
    private IRepository<BookingSeat>? _bookingSeats;
    private IRepository<Payment>? _payments;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<Movie> Movies => _movies ??= new Repository<Movie>(_context);
    public IRepository<Screening> Screenings => _screenings ??= new Repository<Screening>(_context);
    public IRepository<Auditorium> Auditoriums => _auditoriums ??= new Repository<Auditorium>(_context);
    public IRepository<Seat> Seats => _seats ??= new Repository<Seat>(_context);
    public IRepository<Booking> Bookings => _bookings ??= new Repository<Booking>(_context);
    public IRepository<BookingSeat> BookingSeats => _bookingSeats ??= new Repository<BookingSeat>(_context);
    public IRepository<Payment> Payments => _payments ??= new Repository<Payment>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

