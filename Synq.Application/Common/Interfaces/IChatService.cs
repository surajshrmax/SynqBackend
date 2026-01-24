namespace Synq.Application.Common.Interfaces;

public interface IChatService
{
    Task<Guid> CreateOneToOneChatAsync(Guid currentUserId, Guid memberId, CancellationToken cancellationToken);
}