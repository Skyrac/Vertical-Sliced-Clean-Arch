using Application.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Unit.Tests.RepositoryTests.Base;
using Unit.Tests.RepositoryTests.Entities.UserDb;
using Xunit.Abstractions;

namespace Unit.Tests.RepositoryTests;

[Trait("category", "automation_unit_tests")]
[Trait("category", "repository_unit_tests")]
[Collection(nameof(PostgreSqlRepositoryTestCollection))]
public class CountTests(PostgreSqlRepositoryTestDatabaseFixture fixture, ITestOutputHelper outputHelper) 
    : RepositoryTestBase(nameof(CountTests), fixture, outputHelper)
{
    [Fact]
    public async Task Count_ReturnsCorrectCount_Success()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user1 = new User { Id = Guid.NewGuid(), Firstname = "Anna", Lastname = "Smith" };
        var user2 = new User { Id = Guid.NewGuid(), Firstname = "Max", Lastname = "Müller" };
        await userRepository.Add(user1, user2);
        await userRepository.SaveChanges();

        var count = await userRepository.Count(u => u.Firstname.Contains("a"));
        Assert.Equal(2, count);
    }
    
    [Fact]
    public async Task Count_WithEmptyDatabase_ReturnsZero()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();

        var count = await userRepository.Count(_ => true);
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Count_WithPredicateThatMatchesNothing_ReturnsZero()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user1 = new User { Id = Guid.NewGuid(), Firstname = "Anna", Lastname = "Smith" };
        await userRepository.Add(user1);
        await userRepository.SaveChanges();

        var count = await userRepository.Count(u => u.Firstname == "Unbekannt");
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Count_WithoutPredicate_ReturnsTotalCount()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user1 = new User { Id = Guid.NewGuid(), Firstname = "Anna", Lastname = "Smith" };
        var user2 = new User { Id = Guid.NewGuid(), Firstname = "Max", Lastname = "Müller" };
        await userRepository.Add(user1, user2);
        await userRepository.SaveChanges();

        var count = await userRepository.Count(null);
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task Count_WithSelector_ReturnsCountBasedOnProjection()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user1 = new User { Id = Guid.NewGuid(), Firstname = "Anna", Lastname = "Smith" };
        var user2 = new User { Id = Guid.NewGuid(), Firstname = "Max", Lastname = "Müller" };
        await userRepository.Add(user1, user2);
        await userRepository.SaveChanges();

        // Hier wird die Projektion auf den Nachnamen gemacht, sollte die gleiche Anzahl liefern
        var count = await userRepository.Count(_ => true, u => u.Lastname);
        Assert.Equal(2, count);
    }
}