// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Users.Admin;
using offers.itacademy.ge.Domain.Users;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Users.Admins
{
    public class AdminResponseModelsExample : IExamplesProvider<List<AdminResponseModel>>
    {
        public List<AdminResponseModel> GetExamples()
        {
            return new List<AdminResponseModel>
            {
                new AdminResponseModel
                {
                    Id = 1,
                    UserName = "admin1",
                    Email = "admin1@example.com",
                    Role = UserRole.Admin
                },
                new AdminResponseModel
                {
                    Id = 2,
                    UserName = "admin2",
                    Email = "admin2@example.com",
                    Role = UserRole.Admin
                }
            };
        }
    }
}
