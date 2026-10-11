using TaskManagement.Application.DTOs.Auth;

namespace TaskManagement.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(UserRegisterDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(UserLoginDto dto, CancellationToken cancellationToken = default);
}