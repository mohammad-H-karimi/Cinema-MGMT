using Cinema_MGMT.Domain.Enums;

namespace Cinema_MGMT.Application.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid ScreeningId { get; set; }
    public Guid UserId { get; set; }
    public BookingStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public ScreeningDto? Screening { get; set; }
    public List<BookingSeatDto> BookingSeats { get; set; } = new();
    public PaymentDto? Payment { get; set; }
}

