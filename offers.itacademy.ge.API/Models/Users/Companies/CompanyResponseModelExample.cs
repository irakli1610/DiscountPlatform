// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Users.Company;
using offers.itacademy.ge.Domain.Users;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Users.Companies
{
    public class CompanyResponseModelExample : IExamplesProvider<CompanyResponseModel>
    {
        public CompanyResponseModel GetExamples()
        {
            return new CompanyResponseModel
            {
                Id = 1,
                UserName = "TechCorp",
                Email = "contact@techcorp.com",
                Role = UserRole.Company,
                IsActivated = true,
                ImageUrl = "https://example.com/images/techcorp-logo.jpg",
                Balance = 5000.00m
            };
        }
    }
}
