using Microsoft.EntityFrameworkCore;
using Unit.Tests.RepositoryTests.Entities.EmployeeDb;

namespace Unit.Tests.RepositoryTests.Base;

public class EmployeeTestDbContext : DbContext
{
    public EmployeeTestDbContext(DbContextOptions<EmployeeTestDbContext> options) : base(options)
    {
    }

    protected EmployeeTestDbContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("employee_test");
        
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}