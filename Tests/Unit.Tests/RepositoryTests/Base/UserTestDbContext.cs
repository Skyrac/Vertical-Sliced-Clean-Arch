using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Unit.Tests.RepositoryTests.Entities.UserDb;

namespace Unit.Tests.RepositoryTests.Base;

public class UserTestDbContext : DbBaseContext
{
    public UserTestDbContext(DbContextOptions<UserTestDbContext> options, string schema = "public")
        : base(options, schema) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
