using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unit.Tests.RepositoryTests.Entities.EmployeeDb;

public class User
{
    public Guid Id { get; set; }
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
    }
}