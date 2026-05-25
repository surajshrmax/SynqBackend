using Microsoft.AspNetCore.SignalR;
using Synq.Api.Hubs;
using Synq.Application.Common.Interfaces;

namespace Synq.Api.Realtime;

public class RealtimeChatNotifier(IHubContext<ChatHub> context) : IRealtimeChatNotifier
{
  public async Task AddToGroupAsync(string groupId, string connectionId)
  {
    await context.Groups.AddToGroupAsync(connectionId, groupId);
  }

  public async Task RemoveFromGroupAsync(string groupId, string connectionId)
  {
    await context.Groups.RemoveFromGroupAsync(connectionId, groupId);
  }
}
