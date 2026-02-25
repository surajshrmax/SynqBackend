using MediatR;
using Synq.Application.DTOs;

namespace Synq.Application.Features.Message.UpdateMessage;

public record UpdateMessageCommand(string MessageId, string Content) : IRequest;
