using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Synq.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
  public async override Task OnConnectedAsync()
  {
  }
  public async override Task OnDisconnectedAsync(Exception? exception)
  {
  }
}
