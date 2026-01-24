using MediatR;

namespace Synq.Application.Features.Message.SendMessage;

public enum IdType
{
    Chat = 0,
    User = 1
}

public record SendMessageCommand(string Content, string Id, IdType Type) : IRequest<Guid>;