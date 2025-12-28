using System.ComponentModel.DataAnnotations;

namespace Cinema_MGMT.Application.DTOs;

public class UpdateSeatDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid AuditoriumId { get; set; }

    [Required]
    [MaxLength(10)]
    public string Row { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int Number { get; set; }

    public bool IsActive { get; set; }
}

