using System.Linq.Expressions;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Interfaces;

public interface ITaskRepository : IBaseRepository<TaskItem>
{
    Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetByUserIdAndPeriodAsync(Guid userId, Period period, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetByUserIdAndStatusAsync(Guid userId, Status status, CancellationToken cancellationToken = default);
    
    /// <summary>Belirtilen filtreye göre sayfalanmış görevleri ve toplam kayıt sayısını döner.</summary>
    Task<(IReadOnlyList<TaskItem> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<TaskItem, bool>> predicate, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default);
}