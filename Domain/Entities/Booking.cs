using Cinema_MGMT.Domain.Common;
using Cinema_MGMT.Domain.Enums;
using Cinema_MGMT.Domain.Exceptions;
using Cinema_MGMT.Domain.ValueObjects;

namespace Cinema_MGMT.Domain.Entities;

/// <summary>
/// Represents a booking for a movie screening.
/// Encapsulates booking business rules and state transitions.
/// </summary>
public class Booking : BaseEntity
{
    public Guid ScreeningId { get; private set; }
    public Guid UserId { get; private set; }
    public BookingStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime BookingDate { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    // Navigation properties
    public virtual Screening Screening { get; private set; } = null!;
    public virtual ICollection<BookingSeat> BookingSeats { get; private set; } = new List<BookingSeat>();
    public virtual Payment? Payment { get; private set; }

    // Private constructor for EF Core
    private Booking() { }

    /// <summary>
    /// Creates a new booking with pending status.
    /// </summary>
    /// <param name="screeningId">The screening identifier</param>
    /// <param name="userId">The user making the booking</param>
    /// <param name="totalAmount">The total amount for the booking</param>
    /// <param name="expirationMinutes">Minutes until booking expires (default: 15)</param>
    /// <exception cref="BookingDomainException">Thrown when booking rules are violated</exception>
    public Booking(Guid screeningId, Guid userId, decimal totalAmount, int expirationMinutes = 15)
    {
        if (screeningId == Guid.Empty)
            throw new BookingDomainException("Screening ID cannot be empty");

        if (userId == Guid.Empty)
            throw new BookingDomainException("User ID cannot be empty");

        if (totalAmount <= 0)
            throw new BookingDomainException("Total amount must be greater than zero");

        if (expirationMinutes <= 0)
            throw new BookingDomainException("Expiration minutes must be greater than zero");

        ScreeningId = screeningId;
        UserId = userId;
        TotalAmount = totalAmount;
        Status = BookingStatus.Pending;
        BookingDate = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
    }

    /// <summary>
    /// Confirms the booking. Only pending bookings can be confirmed.
    /// </summary>
    /// <exception cref="BookingDomainException">Thrown when booking cannot be confirmed</exception>
    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new BookingDomainException($"Cannot confirm booking with status {Status}. Only pending bookings can be confirmed.");

        if (IsExpired())
            throw new BookingDomainException("Cannot confirm an expired booking");

        Status = BookingStatus.Confirmed;
        MarkAsUpdated();
    }

    /// <summary>
    /// Cancels the booking. Only pending or confirmed bookings can be cancelled.
    /// </summary>
    /// <exception cref="BookingDomainException">Thrown when booking cannot be cancelled</exception>
    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new BookingDomainException("Booking is already cancelled");

        if (Status == BookingStatus.Expired)
            throw new BookingDomainException("Cannot cancel an expired booking");

        Status = BookingStatus.Cancelled;
        MarkAsUpdated();
    }

    /// <summary>
    /// Marks the booking as expired if it has passed the expiration time.
    /// </summary>
    public void MarkAsExpired()
    {
        if (Status == BookingStatus.Expired)
            return;

        if (Status == BookingStatus.Confirmed || Status == BookingStatus.Cancelled)
            throw new BookingDomainException($"Cannot expire booking with status {Status}");

        if (IsExpired())
        {
            Status = BookingStatus.Expired;
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Checks if the booking has expired.
    /// </summary>
    public bool IsExpired()
    {
        return ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the booking can be paid for.
    /// </summary>
    public bool CanBePaid()
    {
        return Status == BookingStatus.Pending && !IsExpired();
    }

    /// <summary>
    /// Adds a seat to the booking.
    /// </summary>
    /// <param name="seatId">The seat identifier</param>
    /// <exception cref="BookingDomainException">Thrown when seat cannot be added</exception>
    public void AddSeat(Guid seatId)
    {
        if (seatId == Guid.Empty)
            throw new BookingDomainException("Seat ID cannot be empty");

        if (Status != BookingStatus.Pending)
            throw new BookingDomainException($"Cannot add seats to booking with status {Status}");

        if (BookingSeats.Any(bs => bs.SeatId == seatId))
            throw new BookingDomainException("Seat is already added to this booking");

        var bookingSeat = new BookingSeat(Id, seatId);
        BookingSeats.Add(bookingSeat);
    }

    /// <summary>
    /// Checks if the booking belongs to the specified user.
    /// </summary>
    public bool BelongsToUser(Guid userId)
    {
        return UserId == userId;
    }
}

