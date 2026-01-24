namespace Synq.Application.Common.Interfaces;

public interface ICurrentUserInterface
{
   public Guid UserId { get; }
   public bool IsAuthenticated { get; }
}