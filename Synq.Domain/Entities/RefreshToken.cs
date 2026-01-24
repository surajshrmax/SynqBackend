namespace Synq.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; private set; }
    public bool IsRevoked { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime ExpiresAt { get; private set; } = DateTime.UtcNow.AddDays(7);
    
    public RefreshToken(){}

    public RefreshToken(Guid userId, string token)
    {
        UserId = userId;
        Token = token;
    }
}