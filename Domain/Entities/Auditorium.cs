using Cinema_MGMT.Domain.Common;

namespace Cinema_MGMT.Domain.Entities;

/// <summary>
/// Represents an auditorium.
/// Encapsulates auditorium business rules and state.
/// </summary>
public class Auditorium : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public virtual ICollection<Screening> Screenings { get; private set; } = new List<Screening>();
    public virtual ICollection<Seat> Seats { get; private set; } = new List<Seat>();

    // Private constructor for EF Core
    private Auditorium() { }

    /// <summary>
    /// Creates a new auditorium.
    /// </summary>
    /// <param name="name">The auditorium name</param>
    /// <param name="capacity">The auditorium capacity</param>
    public Auditorium(string name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero", nameof(capacity));

        Name = name.Trim();
        Capacity = capacity;
        IsActive = true;
    }

    /// <summary>
    /// Updates the auditorium details.
    /// </summary>
    public void Update(string? name = null, int? capacity = null)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name.Trim();

        if (capacity.HasValue && capacity.Value > 0)
            Capacity = capacity.Value;

        MarkAsUpdated();
    }

    /// <summary>
    /// Activates the auditorium.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    /// <summary>
    /// Deactivates the auditorium.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Gets the current number of seats in the auditorium.
    /// </summary>
    public int GetSeatCount()
    {
        return Seats.Count;
    }

    /// <summary>
    /// Checks if the auditorium has available capacity.
    /// </summary>
    public bool HasAvailableCapacity()
    {
        return GetSeatCount() < Capacity;
    }
}

