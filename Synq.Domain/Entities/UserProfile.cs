namespace Synq.Domain.Entities;

public class UserProfile
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string ImageUrl { get; set; }
    public string Bio { get; set; }
    public DateTime LastSeenAt { get; set; }

    public User User { get; private set; } = null!;
    
    public UserProfile(){}

    public UserProfile(Guid userId, string name)
    {
        UserId = userId;
        Name = name;
    }
}