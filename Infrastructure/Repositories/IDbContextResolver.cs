using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public interface IDbContextResolver
{
    DbContext Resolve<TEntity>();
}

public class DbContextResolver : IDbContextResolver
{
    private readonly IEnumerable<IDbContextFactory> _factories;

    public DbContextResolver(IEnumerable<IDbContextFactory> factories)
    {
        _factories = factories;
    }

    public DbContext Resolve<TEntity>()
    {
        foreach (var factory in _factories)
        {
            var context = factory.GetDbContext();
            // Prüfe, ob der DbContext ein Modell für TEntity enthält
            if (context.Model.FindEntityType(typeof(TEntity)) != null)
                return context;
        }

        throw new InvalidOperationException($"Kein passender DbContext für {typeof(TEntity).Name} gefunden.");
    }
}