namespace Cinema_MGMT.Application.DTOs;

public class SeatDto
{
    public Guid Id { get; set; }
    public Guid AuditoriumId { get; set; }
    public string Row { get; set; } = string.Empty;
    public int Number { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

