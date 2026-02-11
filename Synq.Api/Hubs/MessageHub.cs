using System.Collections.Concurrent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Synq.Application.Features.Message.DeleteMessage;
using Synq.Application.Features.Message.SendMessage;
using Synq.Application.Features.Message.UpdateMessage;

namespace Synq.Api.Hubs;

[Authorize]
public class MessageHub(IMediator mediator) : Hub
{
  private readonly ConcurrentDictionary<string, HashSet<string>> ActiveConnections = new();
  public async Task SendMessage(SendMessageCommand command)
  {
    var res = await mediator.Send(command);

    await Clients.Caller.SendAsync("RecieveMessage", res.Message);

    if (ActiveConnections.TryGetValue(res.RecieverId.ToString(), out var connections))
    {
      await Clients.Clients(connections).SendAsync("RecieveMessage", res.Message);
    }
  }

  public async Task UpdateMessage(UpdateMessageCommand updateMessageCommand)
  {
    var (recieverId, msg) = await mediator.Send(updateMessageCommand);
    await Clients.Caller.SendAsync("UpdateDone", msg);

    if (ActiveConnections.TryGetValue(recieverId.ToString(), out var connections))
    {
      await Clients.Clients(connections).SendAsync("MessageUpdate", msg);
    }
  }

  public async Task DeleteMessage(string messageId)
  {
    var res = await mediator.Send(new DeleteMessageCommand(messageId));
    await Clients.All.SendAsync("MessageDeleted", res);
  }

  public async Task AddToChat(string chatId, string userId)
  {
  }

  public async override Task OnConnectedAsync()
  {
    Console.WriteLine($"Client Connected: {Context.ConnectionId}");
    string userId = Context.UserIdentifier!;
    string connectionId = Context.ConnectionId;

    var connections = ActiveConnections.GetOrAdd(userId, _ => new HashSet<string>());

    lock (connections)
    {
      connections.Add(connectionId);
    }

    await base.OnConnectedAsync();
  }

  public async override Task OnDisconnectedAsync(Exception? exception)
  {
    Console.WriteLine($"Client Disconnected: {Context.ConnectionId}");
    string userId = Context.UserIdentifier!;
    string connectionId = Context.ConnectionId;

    if (ActiveConnections.TryGetValue(userId, out var connections))
    {
      lock (connections)
      {
        connections.Remove(connectionId);

        if (connections.Count == 0)
        {
          ActiveConnections.TryRemove(userId, out _);
        }
      }
    }
    await base.OnDisconnectedAsync(exception);
  }
}
