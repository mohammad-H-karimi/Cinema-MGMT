using System.ComponentModel.DataAnnotations;
using Cinema_MGMT.Domain.Enums;

namespace Cinema_MGMT.Application.DTOs;

public class UpdateBookingDto
{
    [Required]
    public Guid Id { get; set; }

    public BookingStatus? Status { get; set; }
}

