using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.Persistance.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.Balance)
                .HasColumnName("Balance")
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.HasMany(c => c.SelectedCategories)
                .WithMany()
                .UsingEntity(j => j.ToTable("CustomerCategories")); // Join table for Customers and Categories

        }
    }
}
