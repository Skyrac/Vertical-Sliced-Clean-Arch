using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unit.Tests.RepositoryTests.Entities.UserDb;

public class User
{
    public Guid Id { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
