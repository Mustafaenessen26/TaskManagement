using FluentValidation;
using TaskManagement.Application.Common.Constants;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Application.Services.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IValidator<UserRegisterDto> _registerValidator;
    private readonly IValidator<UserLoginDto> _loginValidator;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        IValidator<UserRegisterDto> registerValidator,
        IValidator<UserLoginDto> loginValidator)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    public async Task<AuthResponseDto> RegisterAsync(UserRegisterDto dto, CancellationToken cancellationToken = default)
    {
        await _registerValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var email = NormalizeEmail(dto.Email);
        var username = dto.Username.Trim();

        if (await _unitOfWork.Users.ExistsByEmailAsync(email, cancellationToken))
            throw new ConflictException(ErrorMessages.EmailAlreadyExists);

        if (await _unitOfWork.Users.GetByUsernameAsync(username, cancellationToken) is not null)
            throw new ConflictException(ErrorMessages.UsernameAlreadyExists);

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = _passwordHasher.Hash(dto.Password)
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto, CancellationToken cancellationToken = default)
    {
        await _loginValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var user = await _unitOfWork.Users.GetByEmailAsync(NormalizeEmail(dto.Email), cancellationToken);

        // Hangisinin yanlış olduğunu belli etmemek için tek bir mesaj
        if (user is null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedException(ErrorMessages.InvalidCredentials);

        return CreateAuthResponse(user);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private AuthResponseDto CreateAuthResponse(User user)
    {
        var jwt = _tokenGenerator.Generate(user);

        return new AuthResponseDto
        {
            Token = jwt.Token,
            ExpiresAt = jwt.ExpiresAt,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }
}