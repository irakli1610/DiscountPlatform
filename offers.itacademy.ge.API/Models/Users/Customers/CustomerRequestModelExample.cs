// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Users.Customer;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Users.Customers
{
    public class CustomerRequestModelExample : IExamplesProvider<CustomerRequestModel>
    {
        public CustomerRequestModel GetExamples()
        {
            return new CustomerRequestModel
            {
                UserName = "john_doe",
                Email = "john.doe@example.com",
                Password = "CustomerPass123!",
                ConfirmPassword = "CustomerPass123!",
                Balance = 500.00m
            };
        }
    }
}
