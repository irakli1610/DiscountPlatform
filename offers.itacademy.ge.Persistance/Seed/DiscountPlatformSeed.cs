using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using offers.itacademy.ge.Application.Utils;
using offers.itacademy.ge.Domain.Categories;
using offers.itacademy.ge.Domain.ProductOffers;
using offers.itacademy.ge.Domain.Purchases;
using offers.itacademy.ge.Domain.Users;
using offers.itacademy.ge.Persistance.Context;

namespace offers.itacademy.ge.Persistance.Seed
{
    public static class DiscountPlatformSeed
    {
        public static void Initialize(IServiceProvider serviceProviders)
        {
            using var scope = serviceProviders.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<DiscountPlatformContext>();

            Migrate(database);
            SeedEverything(database);
        }

        private static void Migrate(DiscountPlatformContext context)
        {
            context.Database.Migrate();
        }

        private static void SeedEverything(DiscountPlatformContext context)
        {
            SeedCategories(context);
            context.SaveChanges();

            SeedUsers(context);
            context.SaveChanges();

            SeedProductOffers(context);
            context.SaveChanges();

            SeedPurchases(context);
            context.SaveChanges();
        }

        private static void SeedCategories(DiscountPlatformContext context)
        {
            var categories = new List<Category>
            {
                new Category { Name = "Electronics" },
                new Category { Name = "Fashion" },
                new Category { Name = "Home & Kitchen" },
                new Category { Name = "Food & Beverages" },
                new Category { Name = "Sports" }
            };

            foreach (var category in categories)
            {
                if (!context.Categories.Any(c => c.Name == category.Name))
                {
                    context.Categories.Add(category);
                }
            }
        }

        private static void SeedUsers(DiscountPlatformContext context)
        {
            var categories = context.Categories.ToList();

            var users = new List<User>
            {
                // 1 Admin
                new Admin
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    Role = UserRole.Admin,
                    Password = AuthUtils.GeneratePasswordHash("admin123")
                },
                // 2 Companies
                new Company
                {
                    UserName = "techCorp",
                    Email = "techcorp@example.com",
                    Role = UserRole.Company,
                    IsActivated = true,
                    ImageUrl = "defaultCompanyImage.png",
                    Balance = 1500m,
                    Password = AuthUtils.GeneratePasswordHash("tech123")
                },
                new Company
                {
                    UserName = "fashionOutlet",
                    Email = "fashionoutlet@example.com",
                    Role = UserRole.Company,
                    IsActivated = true,
                    ImageUrl = "defaultCompanyImage.png",
                    Balance = 800m,
                    Password = AuthUtils.GeneratePasswordHash("fashion123")
                },
                // 3 Customers
                new Customer
                {
                    UserName = "johnDoe",
                    Email = "johndoe@example.com",
                    Role = UserRole.Customer,
                    Balance = 300m,
                    SelectedCategories = categories.Where(c => c.Name == "Electronics" || c.Name == "Sports").ToList(),
                    Password = AuthUtils.GeneratePasswordHash("john123")
                },
                new Customer
                {
                    UserName = "janeSmith",
                    Email = "janesmith@example.com",
                    Role = UserRole.Customer,
                    Balance = 450m,
                    SelectedCategories = categories.Where(c => c.Name == "Fashion" || c.Name == "Home & Kitchen").ToList(),
                    Password = AuthUtils.GeneratePasswordHash("jane123")
                },
                new Customer
                {
                    UserName = "bobWilson",
                    Email = "bobwilson@example.com",
                    Role = UserRole.Customer,
                    Balance = 200m,
                    SelectedCategories = categories.Where(c => c.Name == "Food & Beverages" || c.Name == "Electronics").ToList(),
                    Password = AuthUtils.GeneratePasswordHash("bob123")
                }
            };

            foreach (var user in users)
            {
                if (!context.Users.Any(u => u.UserName == user.UserName))
                {
                    context.Users.Add(user);
                }
            }
        }

        private static void SeedProductOffers(DiscountPlatformContext context)
        {
            var companies = context.Users.OfType<Company>().ToList();
            var categories = context.Categories.ToList();

            if (!companies.Any() || !categories.Any()) return;

            var random = new Random(); // For random expiration days
            var offers = new List<ProductOffer>
            {
                // TechCorp Offers (Electronics & Sports)
                new ProductOffer
                {
                    Name = "Smartphone X",
                    Description = "Latest 5G smartphone",
                    Price = 599.99m,
                    Quantity = 15,
                    CategoryId = categories.First(c => c.Name == "Electronics").Id,
                    CompanyId = companies.First(c => c.UserName == "techCorp").Id,
                    Status = OfferStatus.Active,
                    CreateTime = DateTime.UtcNow.AddHours(-1),
                    ExpirationTime = DateTime.UtcNow.AddHours(-1).AddDays(random.Next(1, 16)), // 1-15 days
                    ImageUrl = "defaultOfferImage.png"
                },
                new ProductOffer
                {
                    Name = "Wireless Earbuds",
                    Description = "Noise-canceling earbuds",
                    Price = 89.99m,
                    Quantity = 20,
                    CategoryId = categories.First(c => c.Name == "Electronics").Id,
                    CompanyId = companies.First(c => c.UserName == "techCorp").Id,
                    Status = OfferStatus.Active,
                    CreateTime = DateTime.UtcNow.AddMinutes(-30),
                    ExpirationTime = DateTime.UtcNow.AddMinutes(-30).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                },
                new ProductOffer
                {
                    Name = "Fitness Tracker",
                    Description = "Track your workouts",
                    Price = 49.99m,
                    Quantity = 25,
                    CategoryId = categories.First(c => c.Name == "Sports").Id,
                    CompanyId = companies.First(c => c.UserName == "techCorp").Id,
                    Status = OfferStatus.Active,
                    CreateTime = DateTime.UtcNow.AddMinutes(-45),
                    ExpirationTime = DateTime.UtcNow.AddMinutes(-45).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                },
                new ProductOffer
                {
                    Name = "Gaming Console",
                    Description = "Next-gen gaming",
                    Price = 499.99m,
                    Quantity = 10,
                    CategoryId = categories.First(c => c.Name == "Electronics").Id,
                    CompanyId = companies.First(c => c.UserName == "techCorp").Id,
                    Status = OfferStatus.Active,
                    CreateTime = DateTime.UtcNow.AddHours(-2),
                    ExpirationTime = DateTime.UtcNow.AddHours(-2).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                },
                new ProductOffer
                {
                    Name = "Smart Watch",
                    Description = "Health monitoring",
                    Price = 199.99m,
                    Quantity = 12,
                    CategoryId = categories.First(c => c.Name == "Electronics").Id,
                    CompanyId = companies.First(c => c.UserName == "techCorp").Id,
                    Status = OfferStatus.Archived,
                    CreateTime = DateTime.UtcNow.AddDays(-2),
                    ExpirationTime = DateTime.UtcNow.AddDays(-2).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                },
                // FashionOutlet Offers (Fashion & Home & Kitchen)
                new ProductOffer
                {
                    Name = "Leather Jacket",
                    Description = "Stylish winter jacket",
                    Price = 129.99m,
                    Quantity = 8,
                    CategoryId = categories.First(c => c.Name == "Fashion").Id,
                    CompanyId = companies.First(c => c.UserName == "fashionOutlet").Id,
                    Status = OfferStatus.Active,
                    CreateTime = DateTime.UtcNow.AddMinutes(-20),
                    ExpirationTime = DateTime.UtcNow.AddMinutes(-20).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                },
                new ProductOffer
                {
                    Name = "Sneakers",
                    Description = "Trendy casual shoes",
                    Price = 79.99m,
                    Quantity = 15,
                    CategoryId = categories.First(c => c.Name == "Fashion").Id,
                    CompanyId = companies.First(c => c.UserName == "fashionOutlet").Id,
                    Status = OfferStatus.Active,
                    CreateTime = DateTime.UtcNow.AddMinutes(-15),
                    ExpirationTime = DateTime.UtcNow.AddMinutes(-15).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                },
                new ProductOffer
                {
                    Name = "Kitchen Blender",
                    Description = "High-speed blender",
                    Price = 59.99m,
                    Quantity = 20,
                    CategoryId = categories.First(c => c.Name == "Home & Kitchen").Id,
                    CompanyId = companies.First(c => c.UserName == "fashionOutlet").Id,
                    Status = OfferStatus.Active,
                    CreateTime = DateTime.UtcNow.AddHours(-1),
                    ExpirationTime = DateTime.UtcNow.AddHours(-1).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                },
                new ProductOffer
                {
                    Name = "Silk Scarf",
                    Description = "Elegant accessory",
                    Price = 29.99m,
                    Quantity = 30,
                    CategoryId = categories.First(c => c.Name == "Fashion").Id,
                    CompanyId = companies.First(c => c.UserName == "fashionOutlet").Id,
                    Status = OfferStatus.Active,
                    CreateTime = DateTime.UtcNow.AddMinutes(-10),
                    ExpirationTime = DateTime.UtcNow.AddMinutes(-10).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                },
                new ProductOffer
                {
                    Name = "Coffee Maker",
                    Description = "Automatic drip",
                    Price = 39.99m,
                    Quantity = 18,
                    CategoryId = categories.First(c => c.Name == "Home & Kitchen").Id,
                    CompanyId = companies.First(c => c.UserName == "fashionOutlet").Id,
                    Status = OfferStatus.Cancelled,
                    CreateTime = DateTime.UtcNow.AddHours(-3),
                    ExpirationTime = DateTime.UtcNow.AddHours(-3).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                },
                // More TechCorp Offers
                new ProductOffer
                {
                    Name = "Tablet",
                    Description = "Lightweight tablet",
                    Price = 299.99m,
                    Quantity = 10,
                    CategoryId = categories.First(c => c.Name == "Electronics").Id,
                    CompanyId = companies.First(c => c.UserName == "techCorp").Id,
                    Status = OfferStatus.Active,
                    CreateTime = DateTime.UtcNow.AddMinutes(-25),
                    ExpirationTime = DateTime.UtcNow.AddMinutes(-25).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                },
                new ProductOffer
                {
                    Name = "Smart Bulb",
                    Description = "Wi-Fi enabled",
                    Price = 19.99m,
                    Quantity = 50,
                    CategoryId = categories.First(c => c.Name == "Electronics").Id,
                    CompanyId = companies.First(c => c.UserName == "techCorp").Id,
                    Status = OfferStatus.Active,
                    CreateTime = DateTime.UtcNow.AddHours(-1),
                    ExpirationTime = DateTime.UtcNow.AddHours(-1).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                },
                // Mixed Offers
                new ProductOffer
                {
                    Name = "Protein Bars",
                    Description = "Healthy snack",
                    Price = 9.99m,
                    Quantity = 100,
                    CategoryId = categories.First(c => c.Name == "Food & Beverages").Id,
                    CompanyId = companies.First(c => c.UserName == "fashionOutlet").Id,
                    Status = OfferStatus.Active,
                    CreateTime = DateTime.UtcNow.AddMinutes(-5),
                    ExpirationTime = DateTime.UtcNow.AddMinutes(-5).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                },
                new ProductOffer
                {
                    Name = "Yoga Mat",
                    Description = "Non-slip mat",
                    Price = 24.99m,
                    Quantity = 15,
                    CategoryId = categories.First(c => c.Name == "Sports").Id,
                    CompanyId = companies.First(c => c.UserName == "techCorp").Id,
                    Status = OfferStatus.Active,
                    CreateTime = DateTime.UtcNow.AddMinutes(-30),
                    ExpirationTime = DateTime.UtcNow.AddMinutes(-30).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                },
                new ProductOffer
                {
                    Name = "Jeans",
                    Description = "Slim fit jeans",
                    Price = 49.99m,
                    Quantity = 12,
                    CategoryId = categories.First(c => c.Name == "Fashion").Id,
                    CompanyId = companies.First(c => c.UserName == "fashionOutlet").Id,
                    Status = OfferStatus.Active,
                    CreateTime = DateTime.UtcNow.AddHours(-2),
                    ExpirationTime = DateTime.UtcNow.AddHours(-2).AddDays(random.Next(1, 16)),
                    ImageUrl = "defaultOfferImage.png"
                }
            };

            foreach (var offer in offers)
            {
                if (!context.ProductOffers.Any(po => po.Name == offer.Name && po.CompanyId == offer.CompanyId))
                {
                    context.ProductOffers.Add(offer);
                }
            }
        }

        private static void SeedPurchases(DiscountPlatformContext context)
        {
            var customers = context.Users.OfType<Customer>().ToList();
            var offers = context.ProductOffers.ToList();

            if (!customers.Any() || !offers.Any()) return;

            var purchases = new List<Purchase>
            {
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "johnDoe").Id,
                    ProductOfferId = offers.First(o => o.Name == "Smartphone X").Id,
                    Quantity = 1,
                    PurchaseDate = DateTime.UtcNow.AddMinutes(-10),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 599.99m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "janeSmith").Id,
                    ProductOfferId = offers.First(o => o.Name == "Leather Jacket").Id,
                    Quantity = 1,
                    PurchaseDate = DateTime.UtcNow.AddMinutes(-5),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 129.99m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "bobWilson").Id,
                    ProductOfferId = offers.First(o => o.Name == "Protein Bars").Id,
                    Quantity = 5,
                    PurchaseDate = DateTime.UtcNow.AddMinutes(-2),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 49.95m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "johnDoe").Id,
                    ProductOfferId = offers.First(o => o.Name == "Fitness Tracker").Id,
                    Quantity = 2,
                    PurchaseDate = DateTime.UtcNow.AddMinutes(-15),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 99.98m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "janeSmith").Id,
                    ProductOfferId = offers.First(o => o.Name == "Kitchen Blender").Id,
                    Quantity = 1,
                    PurchaseDate = DateTime.UtcNow.AddMinutes(-20),
                    Status = PurchaseStatus.Cancelled,
                    TotalPrice = 59.99m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "bobWilson").Id,
                    ProductOfferId = offers.First(o => o.Name == "Wireless Earbuds").Id,
                    Quantity = 1,
                    PurchaseDate = DateTime.UtcNow.AddMinutes(-25),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 89.99m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "johnDoe").Id,
                    ProductOfferId = offers.First(o => o.Name == "Smart Watch").Id,
                    Quantity = 1,
                    PurchaseDate = DateTime.UtcNow.AddHours(-1),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 199.99m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "janeSmith").Id,
                    ProductOfferId = offers.First(o => o.Name == "Sneakers").Id,
                    Quantity = 1,
                    PurchaseDate = DateTime.UtcNow.AddMinutes(-30),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 79.99m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "bobWilson").Id,
                    ProductOfferId = offers.First(o => o.Name == "Smart Bulb").Id,
                    Quantity = 3,
                    PurchaseDate = DateTime.UtcNow.AddMinutes(-35),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 59.97m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "johnDoe").Id,
                    ProductOfferId = offers.First(o => o.Name == "Gaming Console").Id,
                    Quantity = 1,
                    PurchaseDate = DateTime.UtcNow.AddHours(-2),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 499.99m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "janeSmith").Id,
                    ProductOfferId = offers.First(o => o.Name == "Silk Scarf").Id,
                    Quantity = 2,
                    PurchaseDate = DateTime.UtcNow.AddMinutes(-40),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 59.98m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "bobWilson").Id,
                    ProductOfferId = offers.First(o => o.Name == "Tablet").Id,
                    Quantity = 1,
                    PurchaseDate = DateTime.UtcNow.AddMinutes(-45),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 299.99m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "johnDoe").Id,
                    ProductOfferId = offers.First(o => o.Name == "Yoga Mat").Id,
                    Quantity = 1,
                    PurchaseDate = DateTime.UtcNow.AddMinutes(-50),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 24.99m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "janeSmith").Id,
                    ProductOfferId = offers.First(o => o.Name == "Jeans").Id,
                    Quantity = 1,
                    PurchaseDate = DateTime.UtcNow.AddHours(-1),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 49.99m
                },
                new Purchase
                {
                    CustomerId = customers.First(c => c.UserName == "bobWilson").Id,
                    ProductOfferId = offers.First(o => o.Name == "Coffee Maker").Id,
                    Quantity = 1,
                    PurchaseDate = DateTime.UtcNow.AddHours(-2),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 39.99m
                }
            };

            foreach (var purchase in purchases)
            {
                bool alreadyExists = context.Purchases.Any(p =>
                    p.CustomerId == purchase.CustomerId &&
                    p.ProductOfferId == purchase.ProductOfferId &&
                    p.Quantity == purchase.Quantity &&
                    p.TotalPrice == purchase.TotalPrice &&
                    p.Status == purchase.Status);

                if (!alreadyExists)
                {
                    context.Purchases.Add(purchase);
                }
            }

        }
    }
}