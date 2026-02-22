using MediatR;
using Synq.Application.DTOs.Message;

namespace Synq.Application.Features.Message.GetOlderMessages;

public class GetOlderMessagesQuery : IRequest<MessagePageResponse>
{
    public string ChatId { get; set; }
    public string Cursor { get; set; }
}