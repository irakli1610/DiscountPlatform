// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Users.Customer;
using offers.itacademy.ge.Domain.Users;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Users.Customers
{
    public class CustomerResponseModelsExample : IExamplesProvider<List<CustomerResponseModel>>
    {
        public List<CustomerResponseModel> GetExamples()
        {
            return new List<CustomerResponseModel>
            {
                new CustomerResponseModel
                {
                    Id = 1,
                    UserName = "john_doe",
                    Email = "john.doe@example.com",
                    Role = UserRole.Customer,
                    Balance = 500.00m
                },
                new CustomerResponseModel
                {
                    Id = 2,
                    UserName = "jane_smith",
                    Email = "jane.smith@example.com",
                    Role = UserRole.Customer,
                    Balance = 300.00m
                }
            };
        }
    }
}
