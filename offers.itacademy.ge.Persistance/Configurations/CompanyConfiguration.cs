using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.Persistance.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.Property(c => c.ImageUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(c => c.IsActivated)
                .HasDefaultValue(false);

            builder.Property(c => c.Balance)
                .HasColumnName("Balance")
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);
        }
    }
}
