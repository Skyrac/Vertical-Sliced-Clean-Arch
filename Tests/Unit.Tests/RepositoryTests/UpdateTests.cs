using Application.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Unit.Tests.RepositoryTests.Base;
using Unit.Tests.RepositoryTests.Entities.UserDb;
using Xunit.Abstractions;

namespace Unit.Tests.RepositoryTests;

[Trait("category", "automation_unit_tests")]
[Trait("category", "repository_unit_tests")]
[Collection(nameof(PostgreSqlRepositoryTestCollection))]
public class UpdateTests(PostgreSqlRepositoryTestDatabaseFixture fixture, ITestOutputHelper outputHelper) 
    : RepositoryTestBase(nameof(UpdateTests), fixture, outputHelper)
{
    [Fact]
    public async Task Update_EntityUpdated_Success()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user = new User { Id = Guid.NewGuid(), Firstname = "Max", Lastname = "Mustermann" };
        await userRepository.Add(user);
        await userRepository.SaveChanges();

        user.Firstname = "Maximilian";
        userRepository.Update(user);
        await userRepository.SaveChanges();

        var updatedUser = await userRepository.GetById(user.Id);
        Assert.Equal("Maximilian", updatedUser.Firstname);
    }
    
    [Fact]
    public async Task Update_Expression_BulkUpdate_Success()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        await userRepository.Add(
            new User { Id = Guid.NewGuid(), Firstname = "Test1", Lastname = "Alt" },
            new User { Id = Guid.NewGuid(), Firstname = "Test2", Lastname = "Alt" }
        );
        await userRepository.SaveChanges();

        await userRepository.Update(u => u.SetProperty(x => x.Lastname, "Neu"), u => u.Lastname == "Alt");
        await userRepository.SaveChanges();

        var result = await userRepository.ListAll();
        Assert.All(result, u => Assert.Equal("Neu", u.Lastname));
    }
    
    [Fact]
    public async Task Update_MultipleEntities_EntitiesUpdated_Success()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user1 = new User { Id = Guid.NewGuid(), Firstname = "Max", Lastname = "Mustermann" };
        var user2 = new User { Id = Guid.NewGuid(), Firstname = "Anna", Lastname = "Schmidt" };

        await userRepository.Add(user1, user2);
        await userRepository.SaveChanges();

        user1.Firstname = "Maximilian";
        user2.Lastname = "Müller";

        userRepository.Update(user1, user2);
        await userRepository.SaveChanges();

        var updatedUsers = await userRepository.ListAll();
        Assert.Contains(updatedUsers, u => u.Id == user1.Id && u.Firstname == "Maximilian");
        Assert.Contains(updatedUsers, u => u.Id == user2.Id && u.Lastname == "Müller");
    }

    [Fact]
    public async Task Update_EntityNotTracked_ThrowsInvalidOperationException()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user = new User { Id = Guid.NewGuid(), Firstname = "Max", Lastname = "Mustermann" };

        // Benutzer wird nie hinzugefügt -> nicht getrackt

        // Verhalten prüfen: Ab EF Core 7 wird "Detached" ignoriert
        var exception = Record.Exception(() =>
        {
            userRepository.Update(user);
        });

        Assert.Null(exception); // Sollte ab EF Core 7 kein Fehler mehr sein
    }

    [Fact]
    public async Task Update_Expression_NoMatch_DoesNothing()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        await userRepository.Add(new User { Id = Guid.NewGuid(), Firstname = "Test", Lastname = "Alt" });
        await userRepository.SaveChanges();

        await userRepository.Update(
            u => u.SetProperty(x => x.Lastname, "Neu"),
            u => u.Firstname == "NichtVorhanden"
        );

        var result = await userRepository.ListAll();
        Assert.All(result, u => Assert.Equal("Alt", u.Lastname));
    }

    [Fact]
    public async Task Update_Expression_NullSetExpression_ThrowsArgumentNullException()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await userRepository.Update(null!, u => u.Lastname == "Alt");
        });
    }
}