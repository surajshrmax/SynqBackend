using MediatR;
using Synq.Application.DTOs;
using Synq.Application.DTOs.Message;

namespace Synq.Application.Features.Message.GetMessages;

public record GetMessagesQuery(
    string ChatId,
    bool IsChatId,
    string LastCursorTime, 
    string LastMessageId
    )
    : IRequest<MessagePageResponse>;