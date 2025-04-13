// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Users.Admin;
using offers.itacademy.ge.Domain.Users;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Users.Admins
{
    public class AdminResponseModelExample : IExamplesProvider<AdminResponseModel>
    {
        public AdminResponseModel GetExamples()
        {
            return new AdminResponseModel
            {
                Id = 1,
                UserName = "admin1",
                Email = "admin1@example.com",
                Role = UserRole.Admin
            };
        }
    }
}
