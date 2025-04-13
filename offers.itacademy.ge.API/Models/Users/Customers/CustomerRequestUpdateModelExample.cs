// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Users.Customer;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Users.Customers
{
    public class CustomerRequestUpdateModelExample : IExamplesProvider<CustomerRequestUpdateModel>
    {
        public CustomerRequestUpdateModel GetExamples()
        {
            return new CustomerRequestUpdateModel
            {
                UserName = "john_doe_updated",
                Email = "john.doe.updated@example.com",
                Balance = 600.00m
            };
        }
    }
}
