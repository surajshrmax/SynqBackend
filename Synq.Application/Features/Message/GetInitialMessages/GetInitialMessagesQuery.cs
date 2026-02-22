using MediatR;
using Synq.Application.DTOs;
using Synq.Application.DTOs.Message;

namespace Synq.Application.Features.Message.GetInitialMessages;

public record GetInitialMessagesQuery(
    string ChatId,
    bool IsChatId
    )

    : IRequest<MessagePageResponse>;
