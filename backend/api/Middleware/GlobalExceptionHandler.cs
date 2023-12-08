using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Security;
using System.Security.Authentication;
using api.TransferModels;

namespace api.middelware;

public class GlobalExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext http)
    {
        try
        {
            await _next.Invoke(http);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(http, exception, _logger);
        }
    }

    private static Task HandleExceptionAsync(HttpContext http, Exception exception,
        ILogger<GlobalExceptionHandler> logger)
    {
        http.Response.ContentType = "application/json";
        logger.LogError(exception, "{ExceptionMessage}", exception.Message);

        if (exception is ValidationException ||
            exception is ArgumentException ||
            exception is ArgumentNullException ||
            exception is ArgumentOutOfRangeException)
        {
            http.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        else if (exception is KeyNotFoundException or InvalidCredentialException)//should be the same so users can not see what error is happening when logging in
        {
            http.Response.StatusCode = StatusCodes.Status404NotFound;
            return http.Response.WriteAsJsonAsync(new ResponseDto { MessageToClient = "invalid credentials!" });
        }
        else if (exception is AuthenticationException)
        {
            http.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return http.Response.WriteAsJsonAsync(new ResponseDto { MessageToClient = "You are not Authorised for this action" });
            
        }
        else if (exception is UnauthorizedAccessException)
        {
            http.Response.StatusCode = StatusCodes.Status403Forbidden;
        }
        else if (exception is NotSupportedException ||
                 exception is NotImplementedException)
        {
            http.Response.StatusCode = StatusCodes.Status501NotImplemented;
            return http.Response.WriteAsJsonAsync(new ResponseDto { MessageToClient = "Unable to process request" });
        }
        else if (exception is SecurityException)
        {
            http.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return http.Response.WriteAsJsonAsync(new ResponseDto { MessageToClient = "You are not authorized for this action" });
        }
        else if (exception is SqlNullValueException)
        {
            http.Response.StatusCode = StatusCodes.Status404NotFound;
            return http.Response.WriteAsJsonAsync(new ResponseDto { MessageToClient = "Could not " + exception.Message});
        }
        else if (exception is SqlTypeException)
        {
            http.Response.StatusCode = StatusCodes.Status400BadRequest;
            return http.Response.WriteAsJsonAsync(new ResponseDto { MessageToClient = exception.Message});
        }
        
        else
        {
            http.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return http.Response.WriteAsJsonAsync(new ResponseDto { MessageToClient = "Unable to process request" });
        }

        return http.Response.WriteAsJsonAsync(new ResponseDto
        {
            MessageToClient = exception.Message
        });
    }
}