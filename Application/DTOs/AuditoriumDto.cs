namespace Cinema_MGMT.Application.DTOs;

public class AuditoriumDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

