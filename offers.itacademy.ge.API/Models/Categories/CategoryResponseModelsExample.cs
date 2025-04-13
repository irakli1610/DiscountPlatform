// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Categories;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Categories
{
    public class CategoryResponseModelsExample : IExamplesProvider<List<CategoryResponseModel>>
    {
        public List<CategoryResponseModel> GetExamples()
        {
            return new List<CategoryResponseModel>
            {
            new CategoryResponseModel { Id = 1, Name = "Electronics" },
            new CategoryResponseModel { Id = 2, Name = "Books" },
            new CategoryResponseModel { Id = 3, Name = "Plants" },
            new CategoryResponseModel { Id = 4, Name = "Tickets" },
            new CategoryResponseModel { Id = 5, Name = "Equipment" }
            };
        }
    }
}
