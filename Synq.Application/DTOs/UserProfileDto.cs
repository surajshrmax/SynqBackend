namespace Synq.Application.DTOs;

public class UserProfileDto
{
    public string Name { get; set; }
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? LastSeenAt { get; set; }
}