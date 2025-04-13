// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Users.Admin;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Users.Admins
{
    public class AdminRequestUpdateModelExample : IExamplesProvider<AdminRequestUpdateModel>
    {
        public AdminRequestUpdateModel GetExamples()
        {
            return new AdminRequestUpdateModel
            {
                UserName = "admin1_updated",
                Email = "admin1.updated@example.com"
            };
        }
    }
}
