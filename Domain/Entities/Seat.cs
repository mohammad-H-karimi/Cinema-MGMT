using Cinema_MGMT.Domain.Common;
using Cinema_MGMT.Domain.ValueObjects;

namespace Cinema_MGMT.Domain.Entities;

/// <summary>
/// Represents a seat in an auditorium.
/// Encapsulates seat business rules and state.
/// </summary>
public class Seat : BaseEntity
{
    public Guid AuditoriumId { get; private set; }
    public string Row { get; private set; } = string.Empty;
    public int Number { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public virtual Auditorium Auditorium { get; private set; } = null!;
    public virtual ICollection<BookingSeat> BookingSeats { get; private set; } = new List<BookingSeat>();

    // Private constructor for EF Core
    private Seat() { }

    /// <summary>
    /// Creates a new seat.
    /// </summary>
    /// <param name="auditoriumId">The auditorium identifier</param>
    /// <param name="row">The seat row</param>
    /// <param name="number">The seat number</param>
    public Seat(Guid auditoriumId, string row, int number)
    {
        if (auditoriumId == Guid.Empty)
            throw new ArgumentException("Auditorium ID cannot be empty", nameof(auditoriumId));

        if (string.IsNullOrWhiteSpace(row))
            throw new ArgumentException("Row cannot be null or empty", nameof(row));

        if (number <= 0)
            throw new ArgumentException("Seat number must be greater than zero", nameof(number));

        AuditoriumId = auditoriumId;
        Row = row.Trim().ToUpperInvariant();
        Number = number;
        IsActive = true;
    }

    /// <summary>
    /// Gets the seat number as a value object.
    /// </summary>
    public SeatNumber GetSeatNumber()
    {
        return SeatNumber.Create(Row, Number);
    }

    /// <summary>
    /// Gets the display string for the seat.
    /// </summary>
    public string GetDisplayString()
    {
        return $"{Row}{Number}";
    }

    /// <summary>
    /// Activates the seat.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    /// <summary>
    /// Deactivates the seat.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }
}

