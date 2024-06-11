using PaymentManager.BLL.Exceptions;
using System.Net;

namespace PaymentManager.API.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private const string DefaultResponseMessage = "Internal server error";
    private const HttpStatusCode DefaultHttpStatusCode = HttpStatusCode.InternalServerError;

    public ExceptionHandlerMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = requestDelegate;
        _logger = logger;
    }

    /// <summary>
    ///    Entry point for handling exceptions
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ConnectionNotFoundException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            await HandleExceptionAsync(context, ex.Message,
                responseMessage: string.IsNullOrEmpty(ex.Message)
                    ? DefaultResponseMessage
                    : ex.Message);
        }
    }

    /// <summary>
    ///   This method handles exceptions
    /// </summary>
    /// <param name="context"></param>
    /// <param name="errorMessage"></param>
    /// <param name="httpStatusCode"></param>
    /// <param name="responseMessage"></param>
    /// <returns></returns>
    private static async Task HandleExceptionAsync(HttpContext context, string errorMessage,
        HttpStatusCode httpStatusCode = DefaultHttpStatusCode, string responseMessage = DefaultResponseMessage)
    {
        context.Response.ContentType = "text/plain";
        context.Response.StatusCode = (int)httpStatusCode;

        await context.Response.WriteAsync(responseMessage);
    }
}