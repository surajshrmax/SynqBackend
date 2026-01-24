namespace Synq.Domain.Entities;

public class Chat
{
    public Guid Id { get; set; }

    public bool IsGroup { get; set; }
    public string? Title { get; set; }

    public ICollection<ChatMember> ChatMembers { get; set; }
    public ICollection<Message> Messages { get; set; }

    public DateTime CreatedAt { get; set; }
}