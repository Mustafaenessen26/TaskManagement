using FluentValidation;
using TaskManagement.Application.Common.Constants;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Validators;

public class TaskFilterDtoValidator : AbstractValidator<TaskFilterDto>
{
    public TaskFilterDtoValidator()
    {
        RuleFor(x => x.Period).IsInEnum();

        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(x => x.DateFrom)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
            .WithMessage(ErrorMessages.DateRangeInvalid);
    }
}