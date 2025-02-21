using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.Repositories;

public static class RepositoryRegistrationExtensions
{
    public static IServiceCollection AddRepositories<TDbContext>(this IServiceCollection services)
    where TDbContext : DbContext
    {
        services.TryAddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.TryAddScoped<IDbContextResolver, DbContextResolver>();
        services.AddScoped<IDbContextFactory, DbContextFactory<TDbContext>>();

        return services;
    }
}