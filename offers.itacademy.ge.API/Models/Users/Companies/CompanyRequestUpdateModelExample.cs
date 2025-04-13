// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Users.Company;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Users.Companies
{
    public class CompanyRequestUpdateModelExample : IExamplesProvider<CompanyRequestUpdateModel>
    {
        public CompanyRequestUpdateModel GetExamples()
        {
            return new CompanyRequestUpdateModel
            {
                UserName = "TechCorpUpdated",
                Email = "contact.updated@techcorp.com",
                Balance = 6000.00m
            };
        }
    }
}
