using System.Net;
using UserManager.BLL.Exceptions;

namespace UserManager.Api.Middlewares;

/// <summary>
///     The middleware for handling exceptions.
/// </summary>
public class ExceptionHandlerMiddleware
{
    private const string StandartResponseMessage = "Internal server error";
    private const HttpStatusCode StandartHttpStatusCode = HttpStatusCode.InternalServerError;
    private readonly RequestDelegate _next;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExceptionHandlerMiddleware" /> class.
    /// </summary>
    /// <param name="requestDelegate"></param>
    public ExceptionHandlerMiddleware(RequestDelegate requestDelegate)
    {
        _next = requestDelegate;
    }

    /// <summary>
    ///     The method for handling exceptions.
    /// </summary>
    /// <param name="context"></param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        //TODO inherit from base CustomApiException
        //TODO rewrite with when
        catch (FailedLoginException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (UserAlreadyExistException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.NotFound, ex.Message);
        }
        catch (UserNotFoundException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.NotFound, ex.Message);
        }
        catch (PasswordResetFailedException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (InvalidJwtException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (InvalidRefreshTokenException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (EmailVerificationException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.Forbidden, ex.Message);
        }
        catch (InvalidEmailException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex.Message,
                responseMessage: string.IsNullOrEmpty(ex.Message)
                    ? StandartResponseMessage
                    : ex.Message);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, string errorMessage,
        HttpStatusCode httpStatusCode = StandartHttpStatusCode, string responseMessage = StandartResponseMessage)
    {
        context.Response.ContentType = "text/plain";
        context.Response.StatusCode = (int)httpStatusCode;

        await context.Response.WriteAsync(responseMessage);
    }
}