namespace Synq.Application.DTOs;

public record MessageDto(Guid Id, string Content, bool IsEdited,Guid ChatId, UserDto Sender, Guid SenderId, DateTime SentAt);
