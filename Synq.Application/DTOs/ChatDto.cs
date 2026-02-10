namespace Synq.Application.DTOs;

public record ChatDto(Guid Id, bool IsGroup, string? Title, UserDto? User, MessageDto? LastMessage);