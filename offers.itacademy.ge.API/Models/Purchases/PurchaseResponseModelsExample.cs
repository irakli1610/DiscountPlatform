// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Categories;
using offers.itacademy.ge.Application.Models.ProductOffers;
using offers.itacademy.ge.Application.Models.Purchases;
using offers.itacademy.ge.Application.Models.Users.Company;
using offers.itacademy.ge.Application.Models.Users.Customer;
using offers.itacademy.ge.Domain.ProductOffers;
using offers.itacademy.ge.Domain.Purchases;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Purchases
{
    public class PurchaseResponseModelsExample : IExamplesProvider<List<PurchaseResponseModel>>
    {
        public List<PurchaseResponseModel> GetExamples()
        {
            return new List<PurchaseResponseModel>
            {
                new PurchaseResponseModel
                {
                    Id = 1,
                    CustomerId = 1,
                    Customer = new CustomerResponseModel
                    {
                        Id = 1,
                        UserName = "john_doe",
                        Email = "john.doe@example.com",
                        Balance = 500.00m
                    },
                    ProductOfferId = 1,
                    ProductOffer = new ProductOfferResponseModel
                    {
                        Id = 1,
                        Name = "Laptop",
                        Description = "High-performance gaming laptop",
                        Price = 1200.00m,
                        Quantity = 10,
                        CreateTime = DateTime.UtcNow.AddDays(-5),
                        ExpirationTime = DateTime.UtcNow.AddDays(30),
                        CategoryId = 1,
                        Category = new CategoryResponseModel { Id = 1, Name = "Electronics" },
                        CompanyId = 1,
                        Company = new CompanyResponseModel { Id = 1, UserName = "TechCorp", Email = "contact@techcorp.com", Balance = 5000.00m, IsActivated = true },
                        Status = OfferStatus.Active,
                        ImageUrl = "https://example.com/images/laptop.jpg"
                    },
                    Quantity = 1,
                    PurchaseDate = DateTime.UtcNow.AddHours(-2),
                    Status = PurchaseStatus.Active,
                    TotalPrice = 1200.00m
                },
                new PurchaseResponseModel
                {
                    Id = 2,
                    CustomerId = 2,
                    Customer = new CustomerResponseModel
                    {
                        Id = 2,
                        UserName = "jane_smith",
                        Email = "jane.smith@example.com",
                        Balance = 300.00m
                    },
                    ProductOfferId = 2,
                    ProductOffer = new ProductOfferResponseModel
                    {
                        Id = 2,
                        Name = "Smartphone",
                        Description = "Latest model smartphone with 5G",
                        Price = 800.00m,
                        Quantity = 15,
                        CreateTime = DateTime.UtcNow.AddDays(-3),
                        ExpirationTime = DateTime.UtcNow.AddDays(45),
                        CategoryId = 1,
                        Category = new CategoryResponseModel { Id = 1, Name = "Electronics" },
                        CompanyId = 2,
                        Company = new CompanyResponseModel { Id = 2, UserName = "MobileInc", Email = "support@mobileinc.com", Balance = 3000.00m, IsActivated = true },
                        Status = OfferStatus.Active,
                        ImageUrl = "https://example.com/images/smartphone.jpg"
                    },
                    Quantity = 2,
                    PurchaseDate = DateTime.UtcNow.AddHours(-1),
                    Status = PurchaseStatus.Cancelled,
                    TotalPrice = 1600.00m // Price * Quantity
                }
            };
        }
    }
}
