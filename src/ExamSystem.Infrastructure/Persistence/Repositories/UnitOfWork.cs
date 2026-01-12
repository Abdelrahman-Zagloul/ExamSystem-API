using ExamSystem.Domain.Interfaces;
using ExamSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExamSystem.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExamDbContext _context;
        public UnitOfWork(ExamDbContext context)
        {
            _context = context;
        }
        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
        public async Task<IDbContextTransaction> BeginTransactionAsync() => await _context.Database.BeginTransactionAsync();
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
