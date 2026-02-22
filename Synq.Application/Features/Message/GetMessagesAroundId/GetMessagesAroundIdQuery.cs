using MediatR;
using Synq.Application.DTOs.Message;

namespace Synq.Application.Features.Message.GetMessagesAroundId;

public record GetMessagesAroundIdQuery(string ChatId, string MessageId, string SentAt) : IRequest<MessagePageResponse>;
