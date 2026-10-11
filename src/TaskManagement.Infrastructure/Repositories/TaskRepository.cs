using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskRepository : BaseRepository<TaskItem>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(
        Guid userId, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TaskItem>> GetByUserIdAndPeriodAsync(
        Guid userId, Period period, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Where(t => t.UserId == userId && t.Period == period)
            .OrderByDescending(t => t.Date)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TaskItem>> GetByUserIdAndStatusAsync(
        Guid userId, Status status, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Where(t => t.UserId == userId && t.Status == status)
            .OrderByDescending(t => t.Date)
            .ToListAsync(cancellationToken);

    public async Task<(IReadOnlyList<TaskItem> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<TaskItem, bool>> predicate, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedDate)
            .ThenBy(t => t.Id) // sayfalar arasında kararlı sıralama
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}