namespace Cinema_MGMT.Domain.Exceptions;

/// <summary>
/// Exception thrown when booking domain rules are violated.
/// </summary>
public class BookingDomainException : DomainException
{
    public BookingDomainException(string message) : base(message)
    {
    }
}

