namespace Cinema_MGMT.Application.DTOs;

public class BookingSeatDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid SeatId { get; set; }
    public SeatDto? Seat { get; set; }
}

