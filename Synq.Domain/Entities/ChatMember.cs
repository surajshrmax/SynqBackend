namespace Synq.Domain.Entities;

public class ChatMember
{
    public bool? IsAdmin { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }

    public DateTime CreatedAt { get; set; }
}