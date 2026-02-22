using MediatR;
using Synq.Application.DTOs.Message;

namespace Synq.Application.Features.Message.GetNewerMessages;

public record GetNewerMessagesQuery(string ChatId, string? Cursor) : IRequest<MessagePageResponse>;

