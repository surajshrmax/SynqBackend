namespace Synq.Application.DTOs;

public record MessageDto(Guid Id, string Content, Guid ChatId, UserDto Sender, Guid SenderId, DateTime SentAt);
