using MediatR;
namespace Synq.Application.Features.Message.SendMessage;

public record SendMessageCommand(string LocalId, string Content, string Id, bool IsChat, string? ReplyToMessageId) : IRequest;