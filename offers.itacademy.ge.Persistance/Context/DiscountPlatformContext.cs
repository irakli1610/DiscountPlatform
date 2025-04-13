using Microsoft.EntityFrameworkCore;
using offers.itacademy.ge.Domain.Categories;
using offers.itacademy.ge.Domain.ProductOffers;
using offers.itacademy.ge.Domain.Purchases;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.Persistance.Context
{
    public class DiscountPlatformContext : DbContext
    {
        public DiscountPlatformContext(DbContextOptions<DiscountPlatformContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductOffer> ProductOffers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiscountPlatformContext).Assembly);

        }
    }
}
