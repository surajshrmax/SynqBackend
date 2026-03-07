using System.Net;
using System.Text.Json;
using Synq.Application.Exceptions;

namespace Synq.Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate request, ILogger<ExceptionHandlingMiddleware> logger)
{
  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await request(context);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Unhandled Exception occurred.");
      await HandleExceptionAsync(context, e);
    }
  }

  private static Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    var response = context.Response;
    response.ContentType = "application/json";

    var statusCode = exception switch
    {
      AuthInvalidCredentialsException or AuthUsernameNotAvailableException => HttpStatusCode.BadRequest,
      _ => HttpStatusCode.InternalServerError
    };

    response.StatusCode = (int)statusCode;

    var result = JsonSerializer.Serialize(new
    {
      StatusCode = statusCode,
      message = exception.Message
    });

    return response.WriteAsync(result);
  }
}
