using System.Linq.Expressions;
using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);

    /// <summary>Soft delete: kaydı fiziksel olarak silmez, IsDeleted = true yapar.</summary>
    void Remove(T entity);

    /// <summary>Koşula bağlı veya bağımsız kayıt sayısını veritabanı seviyesinde döner.</summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
}