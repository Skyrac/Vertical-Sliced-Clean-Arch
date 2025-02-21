using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Unit.Tests.RepositoryTests.Base;
public class RepositoryTestBase : IDisposable, IAsyncDisposable
{
    public ServiceProvider ServiceProvider { get; private set; }
    private readonly PostgreSqlRepositoryTestDatabaseFixture _fixture;
    public RepositoryTestBase(string name, PostgreSqlRepositoryTestDatabaseFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        var serviceCollection = new ServiceCollection();
        var connectionString = fixture.Container.GetConnectionString()
            .Replace(PostgreSqlRepositoryTestDatabaseFixture.DefaultDbName, name);
        
        serviceCollection.AddDbContext<UserTestDbContext>(options =>
        {       
            options.UseNpgsql(connectionString);
            options.LogTo(output.WriteLine, LogLevel.Information);
        });
        
        serviceCollection.AddDbContext<EmployeeTestDbContext>(options =>
        {       
            options.UseNpgsql(connectionString);
            options.LogTo(output.WriteLine, LogLevel.Information);        
        });

        serviceCollection.AddRepositories<UserTestDbContext>();
        serviceCollection.AddRepositories<EmployeeTestDbContext>();
        ServiceProvider = serviceCollection.BuildServiceProvider();

        var dbContextFactories = ServiceProvider.GetServices<IDbContextFactory>();

        foreach (var contextFactory in dbContextFactories)
        {
            var context = contextFactory.GetDbContext();

            var schema = context.Model.GetDefaultSchema() ?? "public";

            // Alle Tabellen im Schema droppen (alternativ auch nur das Schema droppen)
            context.Database.ExecuteSqlRaw($"DROP SCHEMA IF EXISTS {schema} CASCADE;");
            context.Database.ExecuteSqlRaw($"CREATE SCHEMA {schema};");

            // EnsureCreated erstellt die Tabellen in dem neuen Schema
            context.Database.ExecuteSqlRaw(context.Database.GenerateCreateScript());
        }

    }

    public void Dispose()
    {
        ServiceProvider.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await ServiceProvider.DisposeAsync();
        await _fixture.DisposeAsync();
    }
}