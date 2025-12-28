using System.ComponentModel.DataAnnotations;

namespace Cinema_MGMT.Application.DTOs;

public class CreateBookingDto
{
    [Required]
    public Guid ScreeningId { get; set; }

    [Required]
    [MinLength(1)]
    public List<Guid> SeatIds { get; set; } = new();
}

