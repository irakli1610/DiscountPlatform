// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Users.Admin;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Users.Admins
{
    public class AdminRequestModelExample : IExamplesProvider<AdminRequestModel>
    {
        public AdminRequestModel GetExamples()
        {
            return new AdminRequestModel
            {
                UserName = "admin1",
                Email = "admin1@example.com",
                Password = "AdminPass123!",
                ConfirmPassword = "AdminPass123!"
            };
        }
    }
}
