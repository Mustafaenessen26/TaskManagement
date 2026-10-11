using TaskManagement.API.Models;

namespace TaskManagement.API.Middleware;

public static class ErrorResponseWriter
{
    public static ErrorResponse Create(
        HttpContext context, int statusCode, string message, IDictionary<string, string[]>? errors)
        => new()
        {
            StatusCode = statusCode,
            Message = message,
            Errors = errors,
            TraceId = context.TraceIdentifier
        };

    public static Task WriteAsync(
        HttpContext context, int statusCode, string message, IDictionary<string, string[]>? errors = null)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(Create(context, statusCode, message, errors));
    }
}