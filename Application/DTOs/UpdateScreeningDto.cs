using System.ComponentModel.DataAnnotations;

namespace Cinema_MGMT.Application.DTOs;

public class UpdateScreeningDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid MovieId { get; set; }

    [Required]
    public Guid AuditoriumId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public bool IsActive { get; set; }
}

