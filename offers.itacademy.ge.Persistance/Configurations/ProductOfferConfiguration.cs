using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using offers.itacademy.ge.Domain.ProductOffers;

namespace offers.itacademy.ge.Persistance.Configurations;

public class ProductOfferConfiguration : IEntityTypeConfiguration<ProductOffer>
{
    public void Configure(EntityTypeBuilder<ProductOffer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.CreateTime).IsRequired();

        builder.Property(x => x.ExpirationTime).IsRequired();
        builder.HasIndex(x => x.ExpirationTime);

        builder.HasOne(x => x.Category)
            .WithMany(c => c.ProductOffers)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Company)
            .WithMany(c => c.ProductOffers)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
