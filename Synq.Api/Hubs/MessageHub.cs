using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Synq.Application.Common.Interfaces;
namespace Synq.Api.Hubs;

[Authorize]
public class MessageHub(IConnectionStore connectionStore) : Hub
{
  public async override Task OnConnectedAsync()
  {
    Console.WriteLine($"Client Connected: {Context.ConnectionId}");
    string userId = Context.UserIdentifier!;
    string connectionId = Context.ConnectionId;

    connectionStore.Add(userId, connectionId);

    await base.OnConnectedAsync();
  }

  public async override Task OnDisconnectedAsync(Exception? exception)
  {
    Console.WriteLine($"Client Disconnected: {Context.ConnectionId}");
    string userId = Context.UserIdentifier!;
    string connectionId = Context.ConnectionId;

    connectionStore.Remove(userId);
    await base.OnDisconnectedAsync(exception);
  }
}
