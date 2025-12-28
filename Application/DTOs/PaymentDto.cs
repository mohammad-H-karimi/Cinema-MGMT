using Cinema_MGMT.Domain.Enums;

namespace Cinema_MGMT.Application.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

