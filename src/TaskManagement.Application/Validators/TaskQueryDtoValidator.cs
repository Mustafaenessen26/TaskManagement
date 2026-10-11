using FluentValidation;
using TaskManagement.Application.Common.Constants;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Validators;

public class TaskQueryDtoValidator : AbstractValidator<TaskQueryDto>
{
    public TaskQueryDtoValidator()
    {
        Include(new TaskFilterDtoValidator());

        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, PaginationConstants.MaxPageSize);
    }
}