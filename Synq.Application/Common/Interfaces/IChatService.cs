using Synq.Domain.Entities;

namespace Synq.Application.Common.Interfaces;

public interface IChatService
{
  public Task<Guid> CreateOneToOneChatAsync(Guid currentUserId, Guid memberId, CancellationToken cancellationToken);

  public Task<Chat?> GetChatAsync(Guid chatId);

  public Task<bool> Exists(Guid chatId);

  public Task<bool> IsAMember(Guid chatId, Guid userId);
}
