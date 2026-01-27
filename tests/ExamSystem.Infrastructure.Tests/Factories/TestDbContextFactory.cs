using ExamSystem.Infrastructure.Persistence.Contexts;
using ExamSystem.Infrastructure.Tests.Persistence.Contexts;
using ExamSystem.Infrastructure.Tests.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Infrastructure.Tests.Factories
{
    public static class TestDbContextFactory
    {
        public static ExamDbContextTest Create()
        {
            var options = new DbContextOptionsBuilder<ExamDbContext>()
           .UseInMemoryDatabase(Guid.NewGuid().ToString())
           .Options;

            var context = new ExamDbContextTest(options);

            context.TestEntities.AddRange(
                new TestEntity { Name = "A" },
                new TestEntity { Name = "B" },
                new TestEntity { Name = "C" }
            );

            context.SaveChanges();

            return context;
        }

    }
}
