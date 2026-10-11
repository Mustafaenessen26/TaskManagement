namespace TaskManagement.Application.Common.Models;

public record JwtToken(string Token, DateTime ExpiresAt);