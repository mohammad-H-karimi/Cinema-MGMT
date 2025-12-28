namespace Cinema_MGMT.Application.DTOs;

public class ScreeningDto
{
    public Guid Id { get; set; }
    public Guid MovieId { get; set; }
    public Guid AuditoriumId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public MovieDto? Movie { get; set; }
    public AuditoriumDto? Auditorium { get; set; }
}

