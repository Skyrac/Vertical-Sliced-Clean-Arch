[assembly: CollectionBehavior(DisableTestParallelization = false, MaxParallelThreads = 4)]

namespace Unit.Tests.RepositoryTests.Base;

[CollectionDefinition(nameof(PostgreSqlRepositoryTestCollection), DisableParallelization = false)]
public class PostgreSqlRepositoryTestCollection : ICollectionFixture<PostgreSqlRepositoryTestDatabaseFixture>
{
    
}