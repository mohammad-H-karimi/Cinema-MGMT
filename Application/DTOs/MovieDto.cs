namespace Cinema_MGMT.Application.DTOs;

public class MovieDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string? PosterUrl { get; set; }
    public decimal TicketPrice { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

