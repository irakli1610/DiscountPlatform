using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.Persistance.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserName).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.UserName);

            builder.Property(x => x.Email).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.Role).IsRequired();


            builder.HasDiscriminator(x => x.Role)
                .HasValue<Admin>(UserRole.Admin)
                .HasValue<Customer>(UserRole.Customer)
                .HasValue<Company>(UserRole.Company);

        }
    }

}
