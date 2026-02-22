namespace Synq.Application.DTOs.Message;

public class MessageCursor
{
    public DateTime SentAt { get; set; }
    public Guid MessageId { get; set; }
}