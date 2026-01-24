namespace Synq.Application.DTOs;

public record MessageDto(Guid Id, string Content, Guid SenderId, DateTime SentAt);
