namespace Synq.Domain.ValueObjects;

public class PasswordHash
{
    public string Value {get;}

    private PasswordHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Password hash is required");
        }

        Value = value;
    }

    public static PasswordHash FromHash(string hash) => new(hash);
}