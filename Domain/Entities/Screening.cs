using Cinema_MGMT.Domain.Common;
using Cinema_MGMT.Domain.Enums;
using Cinema_MGMT.Domain.Exceptions;

namespace Cinema_MGMT.Domain.Entities;

/// <summary>
/// Represents a movie screening in an auditorium.
/// Encapsulates screening business rules and availability checks.
/// </summary>
public class Screening : BaseEntity
{
    public Guid MovieId { get; private set; }
    public Guid AuditoriumId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public virtual Movie Movie { get; private set; } = null!;
    public virtual Auditorium Auditorium { get; private set; } = null!;
    public virtual ICollection<Booking> Bookings { get; private set; } = new List<Booking>();

    // Private constructor for EF Core
    private Screening() { }

    /// <summary>
    /// Creates a new screening.
    /// </summary>
    /// <param name="movieId">The movie identifier</param>
    /// <param name="auditoriumId">The auditorium identifier</param>
    /// <param name="startTime">The screening start time</param>
    /// <param name="endTime">The screening end time</param>
    /// <param name="price">The ticket price</param>
    /// <exception cref="ScreeningDomainException">Thrown when screening rules are violated</exception>
    public Screening(Guid movieId, Guid auditoriumId, DateTime startTime, DateTime endTime, decimal price)
    {
        if (movieId == Guid.Empty)
            throw new ScreeningDomainException("Movie ID cannot be empty");

        if (auditoriumId == Guid.Empty)
            throw new ScreeningDomainException("Auditorium ID cannot be empty");

        if (startTime >= endTime)
            throw new ScreeningDomainException("Start time must be before end time");

        if (startTime < DateTime.UtcNow)
            throw new ScreeningDomainException("Start time cannot be in the past");

        if (price <= 0)
            throw new ScreeningDomainException("Price must be greater than zero");

        MovieId = movieId;
        AuditoriumId = auditoriumId;
        StartTime = startTime;
        EndTime = endTime;
        Price = price;
        IsActive = true;
    }

    /// <summary>
    /// Checks if a seat is available for this screening.
    /// </summary>
    /// <param name="seatId">The seat identifier</param>
    /// <returns>True if the seat is available, false otherwise</returns>
    public bool IsSeatAvailable(Guid seatId)
    {
        if (!IsActive)
            return false;

        // Check if there are any active bookings (pending or confirmed) for this seat
        var activeBookings = Bookings
            .Where(b => b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)
            .ToList();

        foreach (var booking in activeBookings)
        {
            if (booking.BookingSeats.Any(bs => bs.SeatId == seatId))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets all booked seat IDs for this screening.
    /// </summary>
    /// <returns>Collection of booked seat IDs</returns>
    public IEnumerable<Guid> GetBookedSeatIds()
    {
        var activeBookings = Bookings
            .Where(b => b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)
            .ToList();

        return activeBookings
            .SelectMany(b => b.BookingSeats)
            .Select(bs => bs.SeatId)
            .Distinct();
    }

    /// <summary>
    /// Checks if the screening has started.
    /// </summary>
    public bool HasStarted()
    {
        return DateTime.UtcNow >= StartTime;
    }

    /// <summary>
    /// Checks if the screening has ended.
    /// </summary>
    public bool HasEnded()
    {
        return DateTime.UtcNow >= EndTime;
    }

    /// <summary>
    /// Checks if the screening is currently ongoing.
    /// </summary>
    public bool IsOngoing()
    {
        var now = DateTime.UtcNow;
        return now >= StartTime && now < EndTime;
    }

    /// <summary>
    /// Activates the screening.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    /// <summary>
    /// Deactivates the screening.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the screening price.
    /// </summary>
    /// <param name="newPrice">The new price</param>
    /// <exception cref="ScreeningDomainException">Thrown when price is invalid</exception>
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ScreeningDomainException("Price must be greater than zero");

        Price = newPrice;
        MarkAsUpdated();
    }
}

