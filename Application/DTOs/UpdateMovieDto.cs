using System.ComponentModel.DataAnnotations;

namespace Cinema_MGMT.Application.DTOs;

public class UpdateMovieDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }

    [Required]
    [MaxLength(100)]
    public string Genre { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Director { get; set; } = string.Empty;

    [Required]
    public DateTime ReleaseDate { get; set; }

    [MaxLength(500)]
    public string? PosterUrl { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal TicketPrice { get; set; }

    public bool IsActive { get; set; }
}

