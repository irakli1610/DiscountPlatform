// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Categories;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Categories
{
    public class CategoryRequestModelExample : IMultipleExamplesProvider<CategoryRequestModel>
    {
        public IEnumerable<SwaggerExample<CategoryRequestModel>> GetExamples()
        {
            yield return SwaggerExample.Create("Electronics", new CategoryRequestModel
            {
                Name = "Electronics",
            });
            yield return SwaggerExample.Create("Books", new CategoryRequestModel
            {
                Name = "Books",
            });
            yield return SwaggerExample.Create("Plants", new CategoryRequestModel
            {
                Name = "Plants",
            });
            yield return SwaggerExample.Create("Tickets", new CategoryRequestModel
            {
                Name = "Tickets",
            });
            yield return SwaggerExample.Create("Equipment", new CategoryRequestModel
            {
                Name = "Equipment",
            });
        }
    }
}
