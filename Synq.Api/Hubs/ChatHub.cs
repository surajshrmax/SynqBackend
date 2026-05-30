using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Synq.Application.Common.Interfaces;
using Synq.Application.Features.Message.DeleteMessage;
using Synq.Application.Features.Message.SendMessage;
using Synq.Application.Features.Message.UpdateMessage;

namespace Synq.Api.Hubs;

[Authorize]
public class ChatHub(IChatService chatService, IMediator mediator) : Hub
{
  public async override Task OnConnectedAsync()
  {
    await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{Context.UserIdentifier}");
  }
  public async Task JoinChat(string chatId, bool isChat)
  {
    if (isChat)
    {
      var isMember = await chatService.IsAMember(Guid.Parse(chatId), Guid.Parse(Context.UserIdentifier!));

      if (isMember)
      {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
      }
    }
    else
    {
      var chat = await chatService.CreateOneToOneChatAsync(Guid.Parse(Context.UserIdentifier!), Guid.Parse(chatId), CancellationToken.None);

      await Clients.Caller.SendAsync("ChatId", chat);
      await Groups.AddToGroupAsync(Context.ConnectionId, chat.ToString());
    }
  }
  public async Task LeaveChat(string chatId)
  {
    await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
  }

  public async Task SendMessage(string localId, string chatId, string message, string? replyMessageId)
  {
    var dto = await mediator.Send(new SendMessageCommand(localId, message, chatId, replyMessageId));

    await Clients.GroupExcept(chatId, [Context.ConnectionId]).SendAsync("RecieveMessage", dto);
    await Clients.Caller.SendAsync("MessageSent", dto);
  }

  public async Task UpdateMessage(string messageId, string content)
  {
    var result = await mediator.Send(new UpdateMessageCommand(messageId, content));

    await Clients.Group(result.ChatId.ToString()).SendAsync("MessageUpdate", result);
  }

  public async Task DeleteMessage(string messageId)
  {
    var chatId = await mediator.Send(new DeleteMessageCommand(messageId));

    await Clients.Group(chatId.ToString()).SendAsync("MessageDelete", messageId);
  }
}
