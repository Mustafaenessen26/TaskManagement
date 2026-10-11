using TaskManagement.Application.Common.Constants;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs.Tasks;

public class TaskQueryDto : TaskFilterDto
{
    public Status? Status { get; set; }
    public int PageNumber { get; set; } = PaginationConstants.DefaultPageNumber;
    public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
}