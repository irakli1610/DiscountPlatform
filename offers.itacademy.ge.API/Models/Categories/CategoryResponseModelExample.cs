// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Categories;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Categories
{
    public class CategoryResponseModelExample : IExamplesProvider<CategoryResponseModel>
    {
        public CategoryResponseModel GetExamples()
        {
            return new CategoryResponseModel
            {
                Id = 1,
                Name = "Electronics",
            };
        }
    }
}
