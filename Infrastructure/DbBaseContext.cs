using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class DbBaseContext : DbContext
{
    protected string Schema { get; init; }

    public DbBaseContext(DbContextOptions options, string schema = "public")
        : base(options)
    {
        Schema = schema;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        base.OnModelCreating(modelBuilder);
    }
}
