using Synq.Domain.Enums;

namespace Synq.Domain.Entities;

public class Message
{
  public Guid Id { get; set; }

  public string Content { get; set; }
  public MessageType MessageType { get; set; }

  public bool IsEdited { get; set; }

  public Guid SenderId { get; set; }
  public User Sender { get; set; }

  public Guid ChatId { get; set; }
  public Chat Chat { get; set; }

  public DateTime SentAt { get; set; }
}
