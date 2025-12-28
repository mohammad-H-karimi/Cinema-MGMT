using Cinema_MGMT.Domain.Common;

namespace Cinema_MGMT.Domain.Entities;

/// <summary>
/// Represents the relationship between a booking and a seat.
/// This is a join entity in the domain model.
/// </summary>
public class BookingSeat : BaseEntity
{
    public Guid BookingId { get; private set; }
    public Guid SeatId { get; private set; }

    // Navigation properties
    public virtual Booking Booking { get; private set; } = null!;
    public virtual Seat Seat { get; private set; } = null!;

    // Private constructor for EF Core
    private BookingSeat() { }

    /// <summary>
    /// Creates a new booking-seat relationship.
    /// </summary>
    /// <param name="bookingId">The booking identifier</param>
    /// <param name="seatId">The seat identifier</param>
    public BookingSeat(Guid bookingId, Guid seatId)
    {
        if (bookingId == Guid.Empty)
            throw new ArgumentException("Booking ID cannot be empty", nameof(bookingId));

        if (seatId == Guid.Empty)
            throw new ArgumentException("Seat ID cannot be empty", nameof(seatId));

        BookingId = bookingId;
        SeatId = seatId;
    }
}

