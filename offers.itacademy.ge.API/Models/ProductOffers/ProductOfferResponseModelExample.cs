// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Categories;
using offers.itacademy.ge.Application.Models.ProductOffers;
using offers.itacademy.ge.Application.Models.Users.Company;
using offers.itacademy.ge.Domain.ProductOffers;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.ProductOffers
{
    public class ProductOfferResponseModelExample : IExamplesProvider<ProductOfferResponseModel>
    {
        public ProductOfferResponseModel GetExamples()
        {
            return new ProductOfferResponseModel
            {
                Id = 1,
                Name = "Laptop",
                Description = "High-performance gaming laptop with RGB lighting",
                Price = 1200.00m,
                Quantity = 10,
                CreateTime = DateTime.UtcNow.AddDays(-5),
                ExpirationTime = DateTime.UtcNow.AddDays(30),
                CategoryId = 1,
                Category = new CategoryResponseModel
                {
                    Id = 1,
                    Name = "Electronics"
                },
                CompanyId = 1,
                Company = new CompanyResponseModel
                {
                    Id = 1,
                    UserName = "TechCorp",
                    Email = "contact@techcorp.com",
                    Balance = 5000.00m,
                    IsActivated = true
                },
                Status = OfferStatus.Active,
                ImageUrl = "https://example.com/images/laptop.jpg"
            };
        }
    }
}
