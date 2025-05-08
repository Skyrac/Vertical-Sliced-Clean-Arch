using System.Collections.Concurrent;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Unit.Tests.RepositoryTests.Base;

/// <summary>
///     Base class for generic test setup with dependency injection, logging, and database management.
/// </summary>
/// <typeparam name="TDbContext">
///     The type of the core database context used in tests.
///     <example>
///         <see cref="UserTestDbContext" />
///     </example>
/// </typeparam>
public class GenericTestBase<TDbContext> : IAsyncLifetime
    where TDbContext : DbContext
{
    private static readonly ConcurrentBag<string?> Registered = new();
    private ServiceProvider? _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GenericTestBase{TDbContext}" /> class.
    ///     Configures dependency injection, logging, and repositories.
    /// </summary>
    /// <param name="outputHelper">The test output helper for logging.</param>
    protected GenericTestBase(ITestOutputHelper outputHelper)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new XunitLoggerProvider(outputHelper));
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        Services.AddSingleton(loggerFactory);
        Services.AddLogging();
    }

    protected IServiceCollection Services { get; } = new ServiceCollection();
    protected ServiceProvider ServiceProvider
    {
        get => _serviceProvider ??= Services.BuildServiceProvider();
        set => _serviceProvider = value;
    }

    /// <summary>
    ///     Cleans up resources asynchronously.
    /// </summary>
    public virtual async ValueTask DisposeAsync()
    {
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var dbContextFactories = scope.ServiceProvider.GetServices<IDbContextFactory>();
            Task.WaitAll(dbContextFactories.Select(ClearDatabase).ToArray());
        }
        catch (Exception)
        {
            // ignored
        }

        await ServiceProvider.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Initializes the test environment by building the service provider.
    /// </summary>
    public virtual async ValueTask InitializeAsync()
    {
        await BuildServiceProvider();
    }

    /// <summary>
    ///     Builds the service provider and initializes the database.
    /// </summary>
    /// <param name="useMigrate">Indicates whether to apply migrations or just ensure database creation.</param>
    protected virtual async Task BuildServiceProvider(bool useMigrate = true)
    {
        _serviceProvider = Services.BuildServiceProvider();

        using var scope = ServiceProvider.CreateScope();
        var dbContextFactories = scope.ServiceProvider.GetServices<IDbContextFactory>();

        var tasks = new List<Task>();

        foreach (var contextFactory in dbContextFactories)
        {
            await using var context = contextFactory.GetDbContext();

            try
            {
                var connectionString = context.Database.GetDbConnection().ConnectionString;

                if (Registered.Contains(connectionString))
                    continue;
                Registered.Add(connectionString);
                await context.Database.EnsureDeletedAsync();
                tasks.Add(
                    useMigrate
                        ? context.Database.MigrateAsync()
                        : context.Database.EnsureCreatedAsync()
                );
            }
            catch (Exception)
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }
        }

        await Task.WhenAll(tasks);
    }

    /// <summary>
    ///     Clears all data from the database.
    /// </summary>
    /// <param name="contextFactory">The database context factory.</param>
    protected async Task ClearDatabase(IDbContextFactory contextFactory)
    {
        await using var db = contextFactory.GetDbContext();

        lock (db.Database)
        {
            if (db.Database.CanConnect())
            {
                var tableNames = db
                    .Model.GetEntityTypes()
                    .Select(e => new
                    {
                        Schema = e.GetSchema() ?? "public", // default schema fallback
                        Name = e.GetTableName(),
                    })
                    .Where(t => !string.IsNullOrEmpty(t.Name))
                    .Distinct()
                    .Select(t => $"\"{t.Schema}\".\"{t.Name}\"") // wichtig: quotes für case-sensitivity
                    .ToList();

                if (tableNames.Count == 0)
                    return;

                var truncateSql =
                    $"TRUNCATE {string.Join(", ", tableNames)} RESTART IDENTITY CASCADE;";
                db.Database.ExecuteSqlRaw(truncateSql);
            }
        }
    }

    /// <summary>
    ///     Executes an operation using the database context.
    /// </summary>
    /// <param name="operation">The operation to perform on the database context.</param>
    protected virtual void WithDatabase(Action<TDbContext> operation)
    {
        using var scope = ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();

        operation(db);
    }

    /// <summary>
    ///     Seeds the database with data and persists changes.
    /// </summary>
    /// <typeparam name="TADbContext">The type of the database context.</typeparam>
    /// <param name="seeding">The seeding operation.</param>
    public virtual void PersistWithDatabase<TADbContext>(Action<TADbContext> seeding)
        where TADbContext : DbContext
    {
        using var scope = ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TADbContext>();

        seeding(db);

        try
        {
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            Assert.Fail($"{ex.Message}\n\n Inner Exception:\n{ex.InnerException}");
        }
    }

    /// <summary>
    ///     Seeds the database with data using the generic core database context (<see cref="TDbContext" />).
    /// </summary>
    /// <param name="seeding">The seeding operation.</param>
    public virtual void PersistWithDatabase(Action<TDbContext> seeding)
    {
        PersistWithDatabase<TDbContext>(seeding);
    }
}

/// <summary>
///     Extended generic test base class to register MediatR and AutoMapper.
///     <para />
///     Inherits from <see cref="GenericTestBase{TDbContext}" />. <br />
///     <inheritdoc cref="GenericTestBase{TDbContext}" />
/// </summary>
/// <typeparam name="TApplication">
///     The specific implementation of core application to test.
/// </typeparam>
/// <typeparam name="TApi">
///     The interface of core application to test.
/// </typeparam>
/// <typeparam name="TDbContext">
///     The type of the core database context used in tests.
/// </typeparam>
public class GenericTestBase<TApplication, TApi, TDbContext> : GenericTestBase<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GenericTestBase{TApplication, TApi, TDbContext}" /> class.
    ///     Configures MediatR and AutoMapper.
    /// </summary>
    /// <param name="outputHelper">The test output helper.</param>
    protected GenericTestBase(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        Services.AddMediatR(x =>
            x.RegisterServicesFromAssemblies(
                typeof(TApi).Assembly,
                typeof(TApplication).Assembly,
                typeof(TDbContext).Assembly
            )
        );
    }
}
