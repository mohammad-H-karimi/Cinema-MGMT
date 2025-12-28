namespace Cinema_MGMT.Domain.Exceptions;

/// <summary>
/// Exception thrown when payment domain rules are violated.
/// </summary>
public class PaymentDomainException : DomainException
{
    public PaymentDomainException(string message) : base(message)
    {
    }
}

