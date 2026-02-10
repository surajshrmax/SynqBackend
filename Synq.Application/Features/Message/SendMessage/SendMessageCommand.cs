using MediatR;
using Synq.Application.DTOs;
using Synq.Application.DTOs.Message;

namespace Synq.Application.Features.Message.SendMessage;

public enum IdType
{
    Chat = 0,
    User = 1
}

public record SendMessageCommand(string Content, string Id, IdType Type) : IRequest<MessageResponse>;