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
    public class PurchaseResponseModelExample : IExamplesProvider<PurchaseResponseModel>
    {
        public PurchaseResponseModel GetExamples()
        {
            return new PurchaseResponseModel
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
            };
        }
    }
}
