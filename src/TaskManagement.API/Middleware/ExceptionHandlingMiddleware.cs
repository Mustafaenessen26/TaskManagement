using FluentValidation;
using TaskManagement.Application.Common.Constants;
using TaskManagement.Application.Common.Exceptions;

namespace TaskManagement.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // İstemci isteği kendisi iptal etti, yanıt yazmaya gerek yok
            _logger.LogInformation("İstek istemci tarafından iptal edildi: {Path}", context.Request.Path);
        }
        catch (Exception ex) when (!context.Response.HasStarted)
        {
            var (statusCode, message, errors) = Map(ex);

            if (statusCode >= StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Beklenmeyen hata. TraceId: {TraceId}", context.TraceIdentifier);
            else
                _logger.LogWarning("İstek reddedildi ({StatusCode}): {Message}", statusCode, ex.Message);

            await ErrorResponseWriter.WriteAsync(context, statusCode, message, errors);
        }
    }

    private static (int StatusCode, string Message, IDictionary<string, string[]>? Errors) Map(Exception ex) => ex switch
    {
        ValidationException validation => (StatusCodes.Status400BadRequest, ErrorMessages.ValidationFailed, ToErrors(validation)),
        NotFoundException => (StatusCodes.Status404NotFound, ex.Message, null),
        ConflictException => (StatusCodes.Status409Conflict, ex.Message, null),
        UnauthorizedException => (StatusCodes.Status401Unauthorized, ex.Message, null),
        _ => (StatusCodes.Status500InternalServerError, ErrorMessages.UnexpectedError, null)
    };

    private static IDictionary<string, string[]> ToErrors(ValidationException exception)
        => exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray());
}