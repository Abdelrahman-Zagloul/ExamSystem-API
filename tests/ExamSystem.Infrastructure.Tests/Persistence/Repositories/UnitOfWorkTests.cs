using ExamSystem.Infrastructure.Persistence.Repositories;
using ExamSystem.Infrastructure.Tests.Factories;
using ExamSystem.Infrastructure.Tests.Persistence.Entities;

namespace ExamSystem.Infrastructure.Tests.Persistence.Repositories
{
    [Trait("Category", "Infrastructure.Persistence.Repositories.UnitOfWork")]
    public class UnitOfWorkTests
    {
        [Fact]
        public void Repository_ShouldReturnSameInstance_ForSameEntity()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var uow = new UnitOfWork(context);

            // Act
            var repo1 = uow.Repository<TestEntity>();
            var repo2 = uow.Repository<TestEntity>();

            // Assert
            Assert.Same(repo1, repo2);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldPersistChanges()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var uow = new UnitOfWork(context);

            var repo = uow.Repository<TestEntity>();
            await repo.AddAsync(new TestEntity { Id = 4, Name = "Test" });

            // Act
            await uow.SaveChangesAsync();

            // Assert
            Assert.Equal(4, context.TestEntities.Count());
        }
    }
}
