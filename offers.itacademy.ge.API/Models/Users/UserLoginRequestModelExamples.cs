// Copyright (C) TBC Bank. All Rights Reserved.

using offers.itacademy.ge.Application.Models.Users;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Models.Users
{
    public class UserLoginRequestModelExamples : IMultipleExamplesProvider<UserLoginRequestModel>
    {
        public IEnumerable<SwaggerExample<UserLoginRequestModel>> GetExamples()
        {
            yield return SwaggerExample.Create("Admin", new UserLoginRequestModel
            {
                UserName = "admin",
                Password = "admin123"
            });

            yield return SwaggerExample.Create("TechCorp (Company)", new UserLoginRequestModel
            {
                UserName = "techCorp",
                Password = "tech123"
            });

            yield return SwaggerExample.Create("FashionOutlet (Company)", new UserLoginRequestModel
            {
                UserName = "fashionOutlet",
                Password = "fashion123"
            });

            yield return SwaggerExample.Create("JohnDoe (Customer)", new UserLoginRequestModel
            {
                UserName = "johnDoe",
                Password = "john123"
            });

            yield return SwaggerExample.Create("JaneSmith (Customer)", new UserLoginRequestModel
            {
                UserName = "janeSmith",
                Password = "jane123"
            });

            yield return SwaggerExample.Create("BobWilson (Customer)", new UserLoginRequestModel
            {
                UserName = "bobWilson",
                Password = "bob123"
            });
        }
    }
}
