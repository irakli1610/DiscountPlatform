using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using offers.itacademy.ge.Domain.Purchases;

namespace offers.itacademy.ge.Persistance.Configurations
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(x => x.Quantity).IsRequired();
            builder.Property(x => x.PurchaseDate).IsRequired();
            builder.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Purchases)
                .HasForeignKey(x => x.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.ProductOffer)
                .WithMany(po => po.Purchases)
                .HasForeignKey(x => x.ProductOfferId)
                //.IsRequired()
                .OnDelete(DeleteBehavior.SetNull);

        }
    }

}
