using AICommunicationService.BLL.Exceptions;
using System.Net;

namespace AICommunicationService.Middlewares;

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
        catch (DeserializeError ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.OK, ex.Message);
        }
        catch (BalanceException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.PaymentRequired, ex.Message);
        }
        catch (GptModelException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (ToManyRequestsException ex)
        {
            var message = "Rate Limited Exceeded";
            await HandleExceptionAsync(context, message, HttpStatusCode.TooManyRequests, message, retryAfter: ex.Message);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex.Message,
                responseMessage: string.IsNullOrEmpty(ex.Message)
                    ? StandartResponseMessage
                    : ex.Message);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context, string errorMessage,
        HttpStatusCode httpStatusCode = StandartHttpStatusCode, string responseMessage = StandartResponseMessage,
        string? retryAfter = null)
    {
        if (!string.IsNullOrEmpty(retryAfter))
            context.Response.Headers.Add("Retry-After", retryAfter);

        context.Response.ContentType = "text/plain";
        context.Response.StatusCode = (int)httpStatusCode;

        await context.Response.WriteAsync(responseMessage);
    }
}
