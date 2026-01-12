using System.Linq.Expressions;

namespace ExamSystem.Domain.Interfaces
{
    public interface IIGenericRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        IQueryable<TEntity> GetAsQuery();
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> FindAsync(params object[] keyValues);
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes);
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes);
        Task<bool> AnyAsync();
        Task<int> CountAsync();
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter);
    }
}
