using System.Linq.Expressions;
using FluentValidation;
using MapsterMapper;
using TaskManagement.Application.Common.Constants;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Extensions;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Services.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<TaskCreateDto> _createValidator;
    private readonly IValidator<TaskUpdateDto> _updateValidator;
    private readonly IValidator<TaskQueryDto> _queryValidator;
    private readonly IValidator<TaskFilterDto> _filterValidator;

    public TaskService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<TaskCreateDto> createValidator,
        IValidator<TaskUpdateDto> updateValidator,
        IValidator<TaskQueryDto> queryValidator,
        IValidator<TaskFilterDto> filterValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _queryValidator = queryValidator;
        _filterValidator = filterValidator;
    }

    public async Task<TaskDto> CreateAsync(Guid userId, TaskCreateDto dto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var task = _mapper.Map<TaskItem>(dto);
        task.UserId = userId;
        task.Status = Status.Pending;

        await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> GetByIdAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await GetOwnedTaskAsync(userId, taskId, cancellationToken);
        return _mapper.Map<TaskDto>(task);
    }

    public async Task<PagedResult<TaskDto>> GetPagedAsync(
        Guid userId, TaskQueryDto query, CancellationToken cancellationToken = default)
    {
        await _queryValidator.ValidateAndThrowAsync(query, cancellationToken);

        var (from, toExclusive) = NormalizeDateRange(query.DateFrom, query.DateTo);
        var predicate = BuildPredicate(userId, query.Period, query.Status, from, toExclusive);

        var (items, totalCount) = await _unitOfWork.Tasks.GetPagedAsync(
            predicate, query.PageNumber, query.PageSize, cancellationToken);

        return new PagedResult<TaskDto>(
            _mapper.Map<List<TaskDto>>(items), totalCount, query.PageNumber, query.PageSize);
    }

    public async Task<TaskDto> UpdateAsync(
        Guid userId, Guid taskId, TaskUpdateDto dto, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var task = await GetOwnedTaskAsync(userId, taskId, cancellationToken);

        _mapper.Map(dto, task);
        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskDto>(task);
    }

    public async Task DeleteAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await GetOwnedTaskAsync(userId, taskId, cancellationToken);

        _unitOfWork.Tasks.Remove(task); // DbContext bunu soft delete'e çevirir
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<TaskReportDto> GetReportAsync(
        Guid userId, TaskFilterDto filter, CancellationToken cancellationToken = default)
    {
        await _filterValidator.ValidateAndThrowAsync(filter, cancellationToken);

        var (from, toExclusive) = NormalizeDateRange(filter.DateFrom, filter.DateTo);

        var total = await _unitOfWork.Tasks.CountAsync(
            BuildPredicate(userId, filter.Period, null, from, toExclusive), cancellationToken);
        var completed = await _unitOfWork.Tasks.CountAsync(
            BuildPredicate(userId, filter.Period, Status.Completed, from, toExclusive), cancellationToken);
        var inProgress = await _unitOfWork.Tasks.CountAsync(
            BuildPredicate(userId, filter.Period, Status.InProgress, from, toExclusive), cancellationToken);

        return new TaskReportDto
        {
            TotalCount = total,
            CompletedCount = completed,
            InProgressCount = inProgress,
            PendingCount = total - completed - inProgress,
            CompletionRate = total == 0
                ? 0
                : Math.Round((double)completed / total * 100, ReportConstants.RateDecimalPlaces)
        };
    }

    // Başkasının görevi için de NotFound dönüyoruz: görevin var olduğu bilgisi sızmasın
    private async Task<TaskItem> GetOwnedTaskAsync(Guid userId, Guid taskId, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken);

        if (task is null || task.UserId != userId)
            throw new NotFoundException(ErrorMessages.TaskNotFound);

        return task;
    }

    // Tarih filtresi gün bazlı: bitiş günü dahil edilir (dışlayan üst sınır = ertesi gün 00:00)
    private static (DateTime? From, DateTime? ToExclusive) NormalizeDateRange(DateTime? dateFrom, DateTime? dateTo)
        => (dateFrom?.ToUtc().Date, dateTo?.ToUtc().Date.AddDays(1));

    private static Expression<Func<TaskItem, bool>> BuildPredicate(
        Guid userId, Period? period, Status? status, DateTime? from, DateTime? toExclusive)
        => t => t.UserId == userId
             && (period == null || t.Period == period)
             && (status == null || t.Status == status)
             && (from == null || t.Date >= from)
             && (toExclusive == null || t.Date < toExclusive);
}