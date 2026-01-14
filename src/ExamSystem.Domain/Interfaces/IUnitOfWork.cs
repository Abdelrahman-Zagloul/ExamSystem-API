using Microsoft.EntityFrameworkCore.Storage;

namespace ExamSystem.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IIGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
