using ExamSystem.Infrastructure.Persistence.Repositories;
using ExamSystem.Infrastructure.Tests.Factories;
using ExamSystem.Infrastructure.Tests.Persistence.Entities;

namespace ExamSystem.Infrastructure.Tests.Persistence.Repositories;

[Trait("Category", "Infrastructure.Persistence.Repositories.GenericRepository")]
public class GenericRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        //Arrange
        using var context = TestDbContextFactory.Create();
        var repo = new GenericRepository<TestEntity>(context);

        //Act
        await repo.AddAsync(new TestEntity { Name = "D" });
        await context.SaveChangesAsync();

        //Assert
        Assert.Equal(4, await repo.CountAsync());
    }

    [Fact]
    public async Task AddRangeAsync_ShouldAddMultipleEntities()
    {
        //Arrange
        using var context = TestDbContextFactory.Create();
        var repo = new GenericRepository<TestEntity>(context);

        //Act
        await repo.AddRangeAsync(new[]
        {
            new TestEntity { Name = "D" },
            new TestEntity { Name = "E" }
        });
        await context.SaveChangesAsync();

        //Assert
        Assert.Equal(5, await repo.CountAsync());
    }

    [Fact]
    public async Task Remove_ShouldRemoveEntity()
    {
        //Arrange
        using var context = TestDbContextFactory.Create();
        var repo = new GenericRepository<TestEntity>(context);

        //Act
        var entity = await repo.FindAsync(CancellationToken.None, 1);
        repo.Remove(entity!);
        await context.SaveChangesAsync();

        //Assert
        Assert.Equal(2, await repo.CountAsync());
    }

    [Fact]
    public async Task RemoveRange_ShouldRemoveMultipleEntities()
    {
        //Arrange
        using var context = TestDbContextFactory.Create();
        var repo = new GenericRepository<TestEntity>(context);

        //Act
        var entities = repo.GetAsQuery(false).Where(e => e.Name != "C").ToList();
        repo.RemoveRange(entities);
        await context.SaveChangesAsync();

        //Assert
        Assert.Equal(1, await repo.CountAsync());
    }

    [Fact]
    public void GetAsQuery_ShouldReturnQueryable()
    {
        //Arrange
        using var context = TestDbContextFactory.Create();
        var repo = new GenericRepository<TestEntity>(context);

        //Act
        var query = repo.GetAsQuery();

        //Assert
        Assert.Equal(3, query.Count());
    }

    [Fact]
    public async Task FindAsync_ShouldReturnEntityByPrimaryKey_WhenExists()
    {
        //Arrange
        using var context = TestDbContextFactory.Create();
        var repo = new GenericRepository<TestEntity>(context);

        //Act
        var entity = await repo.FindAsync(default, 1);

        //Assert
        Assert.NotNull(entity);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        //Arrange
        using var context = TestDbContextFactory.Create();
        var repo = new GenericRepository<TestEntity>(context);

        //Act
        var result = await repo.GetAllAsync();

        //Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnFilteredEntitiesWithFilter()
    {
        //Arrange
        using var context = TestDbContextFactory.Create();
        var repo = new GenericRepository<TestEntity>(context);

        //Act
        var result = await repo.GetAllAsync(e => e.Name == "B");

        //Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnEntity_WhenExists()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new GenericRepository<TestEntity>(context);

        //Act
        var entity = await repo.GetAsync(e => e.Name == "C");

        //Assert
        Assert.NotNull(entity);
        Assert.Equal("C", entity!.Name);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCount()
    {
        //Arrange
        using var context = TestDbContextFactory.Create();
        var repo = new GenericRepository<TestEntity>(context);

        //Act
        var count = await repo.CountAsync();

        //Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue_WhenAnyExists()
    {

        //Arrange
        using var context = TestDbContextFactory.Create();
        var repo = new GenericRepository<TestEntity>(context);

        //Act
        var exists = await repo.AnyAsync();

        //Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCountWithFilter()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new GenericRepository<TestEntity>(context);

        var count = await repo.CountAsync(e => e.Name != "C");

        //Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task AnyAsync_WithFilter_ShouldReturnTrue_WhenEntityExists()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new GenericRepository<TestEntity>(context);

        var exists = await repo.AnyAsync(e => e.Name == "A");

        //Assert
        Assert.True(exists);
    }
}
