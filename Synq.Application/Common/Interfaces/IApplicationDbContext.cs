using Microsoft.EntityFrameworkCore;
using Synq.Domain.Entities;

namespace Synq.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    
    DbSet<UserProfile> UserProfiles { get; }
    
    DbSet<RefreshToken> RefreshTokens { get; }
    
    DbSet<Chat> Chats { get; }
    
    DbSet<ChatMember> ChatMembers { get; }
    
    DbSet<Message> Messages { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}