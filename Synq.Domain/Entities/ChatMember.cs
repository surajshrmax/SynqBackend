using Synq.Domain.Enums;

namespace Synq.Domain.Entities;

public class ChatMember
{   
    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }

    public GroupRole Role { get; set; }
    
    public GroupPermissions Permissions { get; set; }

    public DateTime CreatedAt { get; set; }
}