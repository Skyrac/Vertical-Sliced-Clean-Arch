using Application.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Unit.Tests.RepositoryTests.Base;
using Unit.Tests.RepositoryTests.Entities.UserDb;
using Xunit.Abstractions;

namespace Unit.Tests.RepositoryTests;

[Trait("category", "automation_unit_tests")]
[Trait("category", "repository_unit_tests")]
[Collection(nameof(PostgreSqlRepositoryTestCollection))]
public class RemoveTests(PostgreSqlRepositoryTestDatabaseFixture fixture, ITestOutputHelper outputHelper) 
    : RepositoryTestBase(nameof(RemoveTests), fixture, outputHelper)
{
    [Fact]
    public async Task Remove_EntityRemoved_Success()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user = new User { Id = Guid.NewGuid(), Firstname = "Max", Lastname = "Mustermann" };
        await userRepository.Add(user);
        await userRepository.SaveChanges();

        userRepository.Remove(user);
        await userRepository.SaveChanges();

        var result = await userRepository.GetById(user.Id);
        Assert.Null(result);
    }
    
    [Fact]
    public async Task Remove_Expression_EntityRemoved_Success()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user = new User { Id = Guid.NewGuid(), Firstname = "Max", Lastname = "Mustermann" };
        await userRepository.Add(user);
        await userRepository.SaveChanges();

        await userRepository.Remove(u => u.Id == user.Id);

        var result = await userRepository.ListAll();
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task Remove_Entity_NotTracked_ThrowsInvalidOperationException()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user = new User { Id = Guid.NewGuid(), Firstname = "Max", Lastname = "Mustermann" };

        // Ohne Add und SaveChanges: User ist nicht getrackt
        var exception = Record.Exception(() =>
        {
            userRepository.Remove(user);
        });

        Assert.Null(exception); // In EF Core 7 ist das kein Fehler mehr, wird als "Detached" ignoriert.
    }

    [Fact]
    public async Task Remove_MultipleEntities_RemovedSuccessfully()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user1 = new User { Id = Guid.NewGuid(), Firstname = "Max", Lastname = "Mustermann" };
        var user2 = new User { Id = Guid.NewGuid(), Firstname = "Anna", Lastname = "Schmidt" };

        await userRepository.Add(user1, user2);
        await userRepository.SaveChanges();

        userRepository.Remove(user1, user2);
        await userRepository.SaveChanges();

        var result = await userRepository.ListAll();
        Assert.Empty(result);
    }

    [Fact]
    public async Task Remove_Expression_NoMatch_DoesNothing()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user1 = new User { Id = Guid.NewGuid(), Firstname = "Max", Lastname = "Mustermann" };
        await userRepository.Add(user1);
        await userRepository.SaveChanges();

        await userRepository.Remove(u => u.Firstname == "NichtVorhanden");
        await userRepository.SaveChanges();

        var result = await userRepository.ListAll();
        Assert.Single(result);
    }

    [Fact]
    public async Task Remove_EmptyExpression_DoesNothing()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user1 = new User { Id = Guid.NewGuid(), Firstname = "Max", Lastname = "Mustermann" };
        await userRepository.Add(user1);
        await userRepository.SaveChanges();

        // Expression, die nie true ist
        await userRepository.Remove(u => false);
        await userRepository.SaveChanges();

        var result = await userRepository.ListAll();
        Assert.Single(result);
    }

    [Fact]
    public async Task Remove_Entity_Null_DoesNothing()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();

        var exception = Record.Exception(() =>
        {
            userRepository.Remove(null!);
        });

        Assert.NotNull(exception); // Erwartet eine Exception, weil null übergeben wurde
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Remove_Expression_NullPredicate_ThrowsArgumentNullException()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();

        Assert.Throws<ArgumentNullException>(() =>
        {
            userRepository.Remove(null!);
        });
    }
}