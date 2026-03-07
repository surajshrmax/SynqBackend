namespace Synq.Application.Exceptions;

public class AuthInvalidCredentialsException : Exception
{
  public AuthInvalidCredentialsException() : base()
  {
  }

  public AuthInvalidCredentialsException(string? message) : base(message)
  {
  }

  public AuthInvalidCredentialsException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
