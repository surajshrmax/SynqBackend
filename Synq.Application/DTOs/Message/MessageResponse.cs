namespace Synq.Application.DTOs.Message;

public record MessageResponse(Guid RecieverId, MessageDto Message);

public record MessagePageResponse(IEnumerable<MessageDto> Messages, DateTime LastCursorTime);