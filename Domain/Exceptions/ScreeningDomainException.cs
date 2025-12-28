namespace Cinema_MGMT.Domain.Exceptions;

/// <summary>
/// Exception thrown when screening domain rules are violated.
/// </summary>
public class ScreeningDomainException : DomainException
{
    public ScreeningDomainException(string message) : base(message)
    {
    }
}

