using System.Net;
using System.Text.Json;
using ESys.Application.Exceptions;

namespace ESys.Api.Middleware;

/// <summary>
/// Middleware for handling custom exceptions in request pipeline
/// </summary>
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await ConvertException(context, ex);
        }
    }

    /// <summary>
    /// Converts exception to corresponding result that can be sent to client
    /// </summary>
    /// <param name="context">Current HttpContext object received from pipeline</param>
    /// <param name="exception">Occured exception to be handled</param>
    /// <returns>Current HttpContext containing exception data and related status code</returns>
    private Task ConvertException(HttpContext context, Exception exception)
    {
        Exception? exc = exception is AggregateException ? exception.InnerException : exception;
        
        HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

        context.Response.ContentType = "application/json";

        var result = string.Empty;

        switch (exc)
        {
            case ValidationException validationException:
                httpStatusCode = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(validationException.ValdationErrors);
                break;
            case BadRequestException badRequestException:
                httpStatusCode = HttpStatusCode.BadRequest;
                result = badRequestException.Message;
                break;
            case NotFoundException:
                httpStatusCode = HttpStatusCode.NotFound;
                break;
            case Exception:
                httpStatusCode = HttpStatusCode.BadRequest;
                break;
        }

        context.Response.StatusCode = (int)httpStatusCode;

        if (result == string.Empty)
        {
            result = JsonSerializer.Serialize(new { error = exception.Message });
        }

        return context.Response.WriteAsync(result);
    }
}