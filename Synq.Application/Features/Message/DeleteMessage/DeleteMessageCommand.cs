using MediatR;

namespace Synq.Application.Features.Message.DeleteMessage;

public record DeleteMessageCommand(string MessageId) : IRequest<Guid>;
