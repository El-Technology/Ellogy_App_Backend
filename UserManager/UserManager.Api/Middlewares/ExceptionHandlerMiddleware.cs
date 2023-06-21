using System.Net;
using UserManager.BLL.Exceptions;

namespace UserManager.Api.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    private const string StandartResponseMessage = "Internal server error";
    private const HttpStatusCode StandartHttpStatusCode = HttpStatusCode.InternalServerError;

    public ExceptionHandlerMiddleware(RequestDelegate requestDelegate)
    {
        _next = requestDelegate;
    }

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
