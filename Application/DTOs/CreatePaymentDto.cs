using System.ComponentModel.DataAnnotations;
using Cinema_MGMT.Domain.Enums;

namespace Cinema_MGMT.Application.DTOs;

public class CreatePaymentDto
{
    [Required]
    public Guid BookingId { get; set; }

    [Required]
    public PaymentMethod Method { get; set; }

    [MaxLength(200)]
    public string? TransactionId { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

