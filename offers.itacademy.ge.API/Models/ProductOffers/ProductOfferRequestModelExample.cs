// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.ProductOffers;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.ProductOffers
{
    public class ProductOfferRequestModelExample : IMultipleExamplesProvider<ProductOfferRequestModel>
    {
        public IEnumerable<SwaggerExample<ProductOfferRequestModel>> GetExamples()
        {
            yield return SwaggerExample.Create("Laptop", new ProductOfferRequestModel
            {
                Name = "Laptop",
                Description = "High-performance gaming laptop",
                Price = 1200.00m,
                Quantity = 10,
                ExpirationTime = DateTime.UtcNow.AddDays(30),
                CategoryId = 1
            });

            yield return SwaggerExample.Create("Smartphone", new ProductOfferRequestModel
            {
                Name = "Smartphone",
                Description = "Latest model smartphone with 5G",
                Price = 800.00m,
                Quantity = 15,
                ExpirationTime = DateTime.UtcNow.AddDays(45),
                CategoryId = 1
            });

            yield return SwaggerExample.Create("Book", new ProductOfferRequestModel
            {
                Name = "Programming Book",
                Description = "Guide to C# programming",
                Price = 29.99m,
                Quantity = 50,
                ExpirationTime = DateTime.UtcNow.AddDays(60),
                CategoryId = 2
            });
        }
    }
}
