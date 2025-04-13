// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Users.Company;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Users.Companies
{
    public class CompanyRequestModelExample : IExamplesProvider<CompanyRequestModel>
    {
        public CompanyRequestModel GetExamples()
        {
            return new CompanyRequestModel
            {
                UserName = "TechCorp",
                Email = "contact@techcorp.com",
                Password = "CompanyPass123!",
                ConfirmPassword = "CompanyPass123!",
                Balance = 5000.00m
            };
        }
    }
}
