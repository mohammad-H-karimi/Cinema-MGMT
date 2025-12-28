using Cinema_MGMT.Domain.Common;
using Cinema_MGMT.Domain.Enums;
using Cinema_MGMT.Domain.Exceptions;

namespace Cinema_MGMT.Domain.Entities;

/// <summary>
/// Represents a payment for a booking.
/// Encapsulates payment business rules and state transitions.
/// </summary>
public class Payment : BaseEntity
{
    public Guid BookingId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public string? TransactionId { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public virtual Booking Booking { get; private set; } = null!;

    // Private constructor for EF Core
    private Payment() { }

    /// <summary>
    /// Creates a new payment with pending status.
    /// </summary>
    /// <param name="bookingId">The booking identifier</param>
    /// <param name="amount">The payment amount</param>
    /// <param name="method">The payment method</param>
    /// <param name="transactionId">Optional transaction identifier</param>
    /// <param name="notes">Optional payment notes</param>
    /// <exception cref="PaymentDomainException">Thrown when payment rules are violated</exception>
    public Payment(Guid bookingId, decimal amount, PaymentMethod method, string? transactionId = null, string? notes = null)
    {
        if (bookingId == Guid.Empty)
            throw new PaymentDomainException("Booking ID cannot be empty");

        if (amount <= 0)
            throw new PaymentDomainException("Payment amount must be greater than zero");

        BookingId = bookingId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;
        PaymentDate = DateTime.UtcNow;
        TransactionId = transactionId;
        Notes = notes;
    }

    /// <summary>
    /// Marks the payment as completed.
    /// </summary>
    /// <param name="transactionId">Optional transaction identifier if not provided during creation</param>
    /// <exception cref="PaymentDomainException">Thrown when payment cannot be marked as completed</exception>
    public void MarkAsPaid(string? transactionId = null)
    {
        if (Status == PaymentStatus.Completed)
            throw new PaymentDomainException("Payment is already completed");

        if (Status == PaymentStatus.Refunded)
            throw new PaymentDomainException("Cannot mark a refunded payment as paid");

        Status = PaymentStatus.Completed;
        
        if (!string.IsNullOrWhiteSpace(transactionId))
        {
            TransactionId = transactionId;
        }

        MarkAsUpdated();
    }

    /// <summary>
    /// Marks the payment as failed.
    /// </summary>
    /// <exception cref="PaymentDomainException">Thrown when payment cannot be marked as failed</exception>
    public void MarkAsFailed()
    {
        if (Status == PaymentStatus.Completed)
            throw new PaymentDomainException("Cannot mark a completed payment as failed");

        if (Status == PaymentStatus.Refunded)
            throw new PaymentDomainException("Cannot mark a refunded payment as failed");

        Status = PaymentStatus.Failed;
        MarkAsUpdated();
    }

    /// <summary>
    /// Marks the payment as refunded.
    /// </summary>
    /// <exception cref="PaymentDomainException">Thrown when payment cannot be refunded</exception>
    public void MarkAsRefunded()
    {
        if (Status != PaymentStatus.Completed)
            throw new PaymentDomainException($"Cannot refund payment with status {Status}. Only completed payments can be refunded.");

        Status = PaymentStatus.Refunded;
        MarkAsUpdated();
    }

    /// <summary>
    /// Checks if the payment is completed.
    /// </summary>
    public bool IsCompleted()
    {
        return Status == PaymentStatus.Completed;
    }

    /// <summary>
    /// Checks if the payment can be refunded.
    /// </summary>
    public bool CanBeRefunded()
    {
        return Status == PaymentStatus.Completed;
    }
}

