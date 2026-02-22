namespace Synq.Application.DTOs.Message;

public record MessageResponse(Guid RecieverId, MessageDto Message);

public class MessagePageResponse
{
  public Guid ChatId { get; set; }
  public bool HasMoreAfter { get; set; }
  public bool HasMoreBefore { get; set; }
  public string? BeforeCursor { get; set; }
  public string? AfterCursor { get; set; }
  public IEnumerable<MessageDto> Messages { get; set; }
};
