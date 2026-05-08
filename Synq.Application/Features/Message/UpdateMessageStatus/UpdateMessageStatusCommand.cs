using MediatR;

namespace Synq.Application.Features.Message.UpdateMessageStatus;

public record UpdateMessageStatusCommand(string MessageId, string Status) : IRequest;