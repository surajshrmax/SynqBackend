using Synq.Domain.ValueObjects;

namespace Synq.Domain.Entities;

public class User
{

    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public UserProfile UserProfile { get; private set; } = null!;
    
    public User(){}

    public User(string username, string email, PasswordHash passwordHash)
    {
        this.Username = username;
        this.Email = email;
        this.PasswordHash = passwordHash;
    }
}