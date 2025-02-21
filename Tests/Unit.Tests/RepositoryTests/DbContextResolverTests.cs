using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Unit.Tests.RepositoryTests.Base;
using Unit.Tests.RepositoryTests.Entities.EmployeeDb;

namespace Unit.Tests.RepositoryTests;

public class DbContextResolverTests
{
    [Fact]
    public void Resolve_UserEntity_ReturnsUserTestDbContext()
    {
        var services = new ServiceCollection();
        var userOptions = new DbContextOptionsBuilder<UserTestDbContext>()
            .UseInMemoryDatabase($"UserDb-{Guid.NewGuid()}")
            .Options;
        var employeeOptions = new DbContextOptionsBuilder<EmployeeTestDbContext>()
            .UseInMemoryDatabase($"EmployeeDb-{Guid.NewGuid()}")
            .Options;

        var userContext = new UserTestDbContext(userOptions);
        var employeeContext = new EmployeeTestDbContext(employeeOptions);

        services.AddScoped<IDbContextFactory>(_ => new DbContextFactory<UserTestDbContext>(userContext));
        services.AddScoped<IDbContextFactory>(_ => new DbContextFactory<EmployeeTestDbContext>(employeeContext));
        var provider = services.BuildServiceProvider();

        var resolver = new DbContextResolver(provider.GetServices<IDbContextFactory>());

        var resolvedContext = resolver.Resolve<RepositoryTests.Entities.UserDb.User>();

        Assert.IsType<UserTestDbContext>(resolvedContext);
    }

    [Fact]
    public void Resolve_EmployeeEntity_ReturnsEmployeeTestDbContext()
    {
        var services = new ServiceCollection();
        var userOptions = new DbContextOptionsBuilder<UserTestDbContext>()
            .UseInMemoryDatabase($"UserDb-{Guid.NewGuid()}")
            .Options;
        var employeeOptions = new DbContextOptionsBuilder<EmployeeTestDbContext>()
            .UseInMemoryDatabase($"EmployeeDb-{Guid.NewGuid()}")
            .Options;

        var userContext = new UserTestDbContext(userOptions);
        var employeeContext = new EmployeeTestDbContext(employeeOptions);

        services.AddScoped<IDbContextFactory>(_ => new DbContextFactory<UserTestDbContext>(userContext));
        services.AddScoped<IDbContextFactory>(_ => new DbContextFactory<EmployeeTestDbContext>(employeeContext));
        var provider = services.BuildServiceProvider();

        var resolver = new DbContextResolver(provider.GetServices<IDbContextFactory>());

        var resolvedContext = resolver.Resolve<Employee>();

        Assert.IsType<EmployeeTestDbContext>(resolvedContext);
    }

    [Fact]
    public void Resolve_UnknownEntity_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection();
        var userOptions = new DbContextOptionsBuilder<UserTestDbContext>()
            .UseInMemoryDatabase($"UserDb-{Guid.NewGuid()}")
            .Options;

        var userContext = new UserTestDbContext(userOptions);

        services.AddScoped<IDbContextFactory>(_ => new DbContextFactory<UserTestDbContext>(userContext));
        var provider = services.BuildServiceProvider();

        var resolver = new DbContextResolver(provider.GetServices<IDbContextFactory>());

        Assert.Throws<InvalidOperationException>(() => resolver.Resolve<Employee>());
    }
}