namespace Synq.Application.Common.Interfaces;

public interface IRealtimeChatNotifier
{
  public Task AddToGroupAsync(string groupId, string connectionId);

  public Task RemoveFromGroupAsync(string groupId, string connectionId);
}

