using System.ComponentModel.DataAnnotations;

namespace Cinema_MGMT.Application.DTOs;

public class CreateAuditoriumDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }
}

