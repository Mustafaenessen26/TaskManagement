using FluentValidation;
using TaskManagement.Application.Common.Constants;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Validators;

public class TaskUpdateDtoValidator : AbstractValidator<TaskUpdateDto>
{
    public TaskUpdateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(ValidationConstants.TaskTitleMaxLength);
        RuleFor(x => x.Description).MaximumLength(ValidationConstants.TaskDescriptionMaxLength);
        RuleFor(x => x.Period).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Date).NotEmpty();
    }
}