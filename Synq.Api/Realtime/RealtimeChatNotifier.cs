using Microsoft.AspNetCore.SignalR;
using Synq.Api.Hubs;
using Synq.Application.Common.Interfaces;

namespace Synq.Api.Realtime;

public class RealtimeChatNotifier(IHubContext<ChatHub> context) : IRealtimeChatNotifier
{

}
