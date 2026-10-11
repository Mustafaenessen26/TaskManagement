using TaskManagement.Application.Common.Models;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Services.Interfaces;

public interface ITaskService
{
    Task<TaskDto> CreateAsync(Guid userId, TaskCreateDto dto, CancellationToken cancellationToken = default);
    Task<TaskDto> GetByIdAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default);
    Task<PagedResult<TaskDto>> GetPagedAsync(Guid userId, TaskQueryDto query, CancellationToken cancellationToken = default);
    Task<TaskDto> UpdateAsync(Guid userId, Guid taskId, TaskUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default);
    Task<TaskReportDto> GetReportAsync(Guid userId, TaskFilterDto filter, CancellationToken cancellationToken = default);
}