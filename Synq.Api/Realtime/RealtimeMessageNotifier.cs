using Microsoft.AspNetCore.SignalR;
using Synq.Api.Hubs;
using Synq.Application.Common.Interfaces;

namespace Synq.Api.Realtime;

public class RealtimeMessageNotifier(IHubContext<MessageHub> messageHub) : IRealTimeMessageNotifier
{
  public async Task SendToUserAsync(string userId, string method, object data)
  {
    await messageHub.Clients.Client(userId).SendAsync(method, data);
  }
}
