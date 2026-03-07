namespace Synq.Application.Exceptions;

public class AuthUsernameNotAvailableException : Exception
{
  public AuthUsernameNotAvailableException() : base()
  {
  }

  public AuthUsernameNotAvailableException(string? message) : base(message)
  {
  }

  public AuthUsernameNotAvailableException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
