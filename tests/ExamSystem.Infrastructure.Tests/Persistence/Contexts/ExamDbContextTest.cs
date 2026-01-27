using ExamSystem.Infrastructure.Persistence.Contexts;
using ExamSystem.Infrastructure.Tests.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Infrastructure.Tests.Persistence.Contexts
{
    public class ExamDbContextTest : ExamDbContext
    {
        public ExamDbContextTest(DbContextOptions<ExamDbContext> options)
          : base(options)
        {
        }

        public DbSet<TestEntity> TestEntities => Set<TestEntity>();
    }
}
