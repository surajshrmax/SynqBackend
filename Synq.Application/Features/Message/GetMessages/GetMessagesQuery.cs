using MediatR;
using Synq.Application.DTOs;

namespace Synq.Application.Features.Message.GetMessages;

public record GetMessagesQuery(string ChatId) : IRequest<IEnumerable<MessageDto>>;