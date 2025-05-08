using Testcontainers.PostgreSql;
using Unit.Tests;

[assembly: AssemblyFixture(typeof(PostgreSqlRepositoryTestDatabaseFixture))]
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Unit.Tests;

public class PostgreSqlRepositoryTestDatabaseFixture : IAsyncLifetime
{
    public static string DefaultDbName = "";
    private const string DatabaseUsername = "root";
    private const string DatabasePassword = "rootpw";
    public readonly PostgreSqlContainer Container;

    public PostgreSqlRepositoryTestDatabaseFixture()
    {
        DefaultDbName = Guid.NewGuid().ToString();
        Container = new PostgreSqlBuilder()
            .WithName(DefaultDbName)
            .WithUsername(DatabaseUsername)
            .WithPassword(DatabasePassword)
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await Container.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Container.StopAsync();
    }
}
