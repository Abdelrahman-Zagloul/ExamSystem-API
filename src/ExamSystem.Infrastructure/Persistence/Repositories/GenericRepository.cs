using ExamSystem.Domain.Interfaces;
using ExamSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExamSystem.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity> : IIGenericRepository<TEntity> where TEntity : class
    {
        private readonly ExamDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        public GenericRepository(ExamDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);
        public async Task AddRangeAsync(IEnumerable<TEntity> entities) => await _dbSet.AddRangeAsync(entities);

        public void Remove(TEntity entity) => _dbSet.Remove(entity);
        public void RemoveRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

        public async Task<bool> AnyAsync() => await _dbSet.AsNoTracking().AnyAsync();
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter) => await _dbSet.AsNoTracking().AnyAsync(filter);

        public async Task<int> CountAsync() => await _dbSet.AsNoTracking().CountAsync();
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter) => await _dbSet.AsNoTracking().CountAsync(filter);

        public IQueryable<TEntity> GetAsQuery() => _dbSet.AsQueryable();
        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<TEntity?> FindAsync(params object[] keyValues) => await _dbSet.FindAsync(keyValues);
        public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);
            return query.FirstOrDefaultAsync();
        }
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);


            return query;
        }
    }
}
