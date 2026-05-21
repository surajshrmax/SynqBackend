namespace Synq.Application.Common.Interfaces;

public interface IRealTimeMessageNotifier
{
  public Task SendToUserAsync(string userId, string method, object data);
  public Task SendToGroupAsync(string groupId, string method, object data);
}
