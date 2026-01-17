using System.Linq.Expressions;

namespace ExamSystem.Domain.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

        IQueryable<TEntity> GetAsQuery(bool asNoTracking = true);
        Task<TEntity?> FindAsync(CancellationToken cancellationToken = default, params object[] keyValues);
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);
        Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);

        Task<bool> AnyAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
    }
}
