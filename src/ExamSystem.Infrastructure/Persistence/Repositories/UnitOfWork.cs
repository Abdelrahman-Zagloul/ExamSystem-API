using ExamSystem.Domain.Interfaces;
using ExamSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExamSystem.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExamDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(ExamDbContext context)
        {
            _context = context;
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);
        public async Task<IDbContextTransaction> BeginTransactionAsync()
            => await _context.Database.BeginTransactionAsync();
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var entityType = typeof(TEntity);
            if (_repositories.TryGetValue(entityType, out var repo))
                return (IGenericRepository<TEntity>)repo;

            var newRepo = new GenericRepository<TEntity>(_context);
            _repositories[entityType] = newRepo;
            return newRepo;
        }
    }
}
