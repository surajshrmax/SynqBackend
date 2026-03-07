using MediatR;

namespace Synq.Application.Features.Message.NotifyTyping;

public record NotifyTypingCommand(string ChatId, bool IsTyping) : IRequest;

