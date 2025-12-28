namespace Cinema_MGMT.Domain.ValueObjects;

/// <summary>
/// Value object representing a seat number (row + number).
/// Ensures seat number is valid.
/// </summary>
public sealed class SeatNumber : IEquatable<SeatNumber>
{
    public string Row { get; }
    public int Number { get; }

    private SeatNumber(string row, int number)
    {
        if (string.IsNullOrWhiteSpace(row))
            throw new ArgumentException("Row cannot be null or empty", nameof(row));

        if (number <= 0)
            throw new ArgumentException("Seat number must be greater than zero", nameof(number));

        Row = row.Trim().ToUpperInvariant();
        Number = number;
    }

    public static SeatNumber Create(string row, int number)
    {
        return new SeatNumber(row, number);
    }

    public string ToDisplayString()
    {
        return $"{Row}{Number}";
    }

    public bool Equals(SeatNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Row == other.Row && Number == other.Number;
    }

    public override bool Equals(object? obj)
    {
        return obj is SeatNumber other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Number);
    }

    public static bool operator ==(SeatNumber? left, SeatNumber? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(SeatNumber? left, SeatNumber? right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return ToDisplayString();
    }
}

