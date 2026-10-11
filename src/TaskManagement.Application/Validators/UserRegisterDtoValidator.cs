using FluentValidation;
using TaskManagement.Application.Common.Constants;
using TaskManagement.Application.DTOs.Auth;

namespace TaskManagement.Application.Validators;

public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
{
    public UserRegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(ValidationConstants.UsernameMinLength, ValidationConstants.UsernameMaxLength)
            .Matches(ValidationConstants.UsernamePattern).WithMessage(ErrorMessages.UsernameInvalidFormat);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(ValidationConstants.EmailMaxLength)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(ValidationConstants.PasswordMinLength)
            .MaximumLength(ValidationConstants.PasswordMaxLength)
            .Matches("[A-Z]").WithMessage(ErrorMessages.PasswordRequiresUppercase)
            .Matches("[a-z]").WithMessage(ErrorMessages.PasswordRequiresLowercase)
            .Matches("[0-9]").WithMessage(ErrorMessages.PasswordRequiresDigit);
    }
}