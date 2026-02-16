namespace Synq.Application.DTOs;

public class MessageDto
{
  public Guid Id { get; set; }
  public string Content { get; set; }
  public bool IsEdited { get; set; }
  public Guid? ReplyMessageId { get; set; }
  public MessageDto? Reply { get; set; }
  public Guid ChatId { get; set; }
  public UserDto Sender { get; set; }
  public Guid SenderId { get; set; }
  public DateTime SentAt { get; set; }
}
