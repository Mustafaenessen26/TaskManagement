namespace TaskManagement.API.Models;

public class ErrorResponse
{
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; init; }
    public string TraceId { get; init; } = string.Empty;
}