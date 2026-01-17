using ExamSystem.Domain.Interfaces;
using ExamSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExamSystem.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        private readonly ExamDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ExamDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await _dbSet.AddAsync(entity, cancellationToken);

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            => await _dbSet.AddRangeAsync(entities, cancellationToken);

        public void Remove(TEntity entity) => _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

        public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().AnyAsync(cancellationToken);

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().AnyAsync(filter, cancellationToken);

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().CountAsync(cancellationToken);

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().CountAsync(filter, cancellationToken);

        public IQueryable<TEntity> GetAsQuery(bool asNoTracking = true)
            => asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsQueryable();

        public async Task<TEntity?> FindAsync(CancellationToken cancellationToken = default, params object[] keyValues)
            => await _dbSet.FindAsync(keyValues, cancellationToken);

        public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable().AsNoTracking();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);
            return query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable().AsNoTracking();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);


            return await query.ToListAsync();
        }
    }
}
