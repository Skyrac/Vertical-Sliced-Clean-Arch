using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

namespace Unit.Tests;

public class PostgreSqlRepositoryTestDatabaseFixture : IAsyncLifetime
{
    public const string DefaultDbName = "test";    
    private const string DatabaseUsername = "root";
    private const string DatabasePassword = "rootpw";
    public readonly PostgreSqlContainer Container;

    public PostgreSqlRepositoryTestDatabaseFixture()
    {
        Container = new PostgreSqlBuilder()
            .WithName(DefaultDbName)
            .WithUsername(DatabaseUsername)
            .WithPassword(DatabasePassword)
            .Build();

    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
    }
}