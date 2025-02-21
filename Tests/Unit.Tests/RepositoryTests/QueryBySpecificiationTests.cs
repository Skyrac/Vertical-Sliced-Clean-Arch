using Application.Repositories;
using Infrastructure.Repositories.Specification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Unit.Tests.RepositoryTests.Base;
using Unit.Tests.RepositoryTests.Entities.UserDb;
using Xunit.Abstractions;

namespace Unit.Tests.RepositoryTests;

[Trait("category", "automation_unit_tests")]
[Trait("category", "repository_unit_tests")]
[Collection(nameof(PostgreSqlRepositoryTestCollection))]
public class QueryBySpecificiationTests(PostgreSqlRepositoryTestDatabaseFixture fixture, ITestOutputHelper outputHelper) 
    : RepositoryTestBase(nameof(QueryBySpecificiationTests), fixture, outputHelper)
{
    class UserByFirstnameSpecification : BaseSpecification<User>
    {
        public UserByFirstnameSpecification(string firstname)
        {
            ApplyCriteria(u => u.Firstname == firstname);
        }
    }
    
    [Fact]
    public async Task QueryBySpecification_ReturnsFilteredResult_Success()
    {

        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user1 = new User { Id = Guid.NewGuid(), Firstname = "Anna", Lastname = "Smith" };
        var user2 = new User { Id = Guid.NewGuid(), Firstname = "Max", Lastname = "Müller" };
        await userRepository.Add(user1, user2);
        await userRepository.SaveChanges();

        var spec = new UserByFirstnameSpecification("Anna");
        var result = await userRepository.QueryBySpecification(spec);

        Assert.Single(result);
        Assert.Equal("Anna", result[0].Firstname);
    }
    
    [Fact]
    public async Task QueryBySpecification_WithNoMatch_ReturnsEmptyResult()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();

        var spec = new UserByFirstnameSpecification("Unbekannt");
        var result = await userRepository.QueryBySpecification(spec);

        Assert.Empty(result);
    }
    
    [Fact]
    public async Task QueryBySpecification_WithEmptyDatabase_ReturnsEmptyResult()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();

        var spec = new UserByFirstnameSpecification("Anna");
        var result = await userRepository.QueryBySpecification(spec);

        Assert.Empty(result);
    }
    
    class UsersOrderedByFirstnameSpecification : BaseSpecification<User>
    {
        public UsersOrderedByFirstnameSpecification()
        {
            ApplyOrder(true, u => u.Firstname);
        }
    }

    [Fact]
    public async Task QueryBySpecification_WithOrderByAscending_ReturnsOrderedResult()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user1 = new User { Id = Guid.NewGuid(), Firstname = "Zoe", Lastname = "Müller" };
        var user2 = new User { Id = Guid.NewGuid(), Firstname = "Anna", Lastname = "Smith" };
        await userRepository.Add(user1, user2);
        await userRepository.SaveChanges();

        var spec = new UsersOrderedByFirstnameSpecification();
        var result = await userRepository.QueryBySpecification(spec);

        Assert.Equal(2, result.Count);
        Assert.Equal("Anna", result[0].Firstname);
        Assert.Equal("Zoe", result[1].Firstname);
    }
    
    class UsersOrderedByFirstnameDescendingSpecification : BaseSpecification<User>
    {
        public UsersOrderedByFirstnameDescendingSpecification()
        {
            ApplyOrder(false, u => u.Firstname);
        }
    }

    [Fact]
    public async Task QueryBySpecification_WithOrderByDescending_ReturnsOrderedResult()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user1 = new User { Id = Guid.NewGuid(), Firstname = "Anna", Lastname = "Smith" };
        var user2 = new User { Id = Guid.NewGuid(), Firstname = "Zoe", Lastname = "Müller" };
        await userRepository.Add(user1, user2);
        await userRepository.SaveChanges();

        var spec = new UsersOrderedByFirstnameDescendingSpecification();
        var result = await userRepository.QueryBySpecification(spec);

        Assert.Equal(2, result.Count);
        Assert.Equal("Zoe", result[0].Firstname);
        Assert.Equal("Anna", result[1].Firstname);
    }
    
    class UserWithOrdersSpecification : BaseSpecification<User>
    {
        public UserWithOrdersSpecification()
        {
            AddInclude(u => u.Include(x => x.Orders));
        }
    }

    [Fact]
    public async Task QueryBySpecification_WithInclude_ReturnsRelatedEntities()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user = new User { Id = Guid.NewGuid(), Firstname = "Anna", Lastname = "Smith", 
            Orders = [new Order { Id = Guid.NewGuid(), Description = "Test Order" }]};

        await userRepository.Add(user);
        await userRepository.SaveChanges();

        var spec = new UserWithOrdersSpecification();
        var result = await userRepository.QueryBySpecification(spec);

        Assert.Single(result);
        Assert.Single(result[0].Orders);
        Assert.Equal("Test Order", result[0].Orders.First().Description);
    }
    
    [Fact]
    public async Task QueryBySpecification_WithSelector_ReturnsProjectedResult()
    {
        var userRepository = ServiceProvider.GetRequiredService<IRepository<User>>();
        var user1 = new User { Id = Guid.NewGuid(), Firstname = "Anna", Lastname = "Smith" };
        await userRepository.Add(user1);
        await userRepository.SaveChanges();

        var spec = new UserByFirstnameSpecification("Anna");
        var result = await userRepository.QueryBySpecification(spec, u => u.Firstname);

        Assert.Single(result);
        Assert.Equal("Anna", result.First());
    }
    
}