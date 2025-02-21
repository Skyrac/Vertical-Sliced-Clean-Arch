using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unit.Tests.RepositoryTests.Entities.UserDb;

public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Description { get; set; }
    public User? Customer { get; set; }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.CustomerId)
            .IsRequired();
    }
}