using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace offers.itacademy.ge.API.Infrastructure.Extensions
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerDocs(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // Add API versions
                options.SwaggerDoc("v1", CreateOpenApiInfo("v1", "Discount Products Management API v1"));
                options.SwaggerDoc("v2", CreateOpenApiInfo("v2", "Discount Products Management API v2"));

                // Add JWT Authentication to Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer {your JWT token}' to authenticate."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                // Enable XML Comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }

                // add Swagger examples
                options.ExampleFilters();

            });

            services.AddSwaggerExamplesFromAssemblyOf<Program>();
        }

        private static OpenApiInfo CreateOpenApiInfo(string version, string title)
        {
            return new OpenApiInfo
            {
                Title = title,
                Version = version,
                Description = $"{title} with global exception handling, versioning, and fluent validation.",
                Contact = new OpenApiContact
                {
                    Name = "DiscountProductsAPI",
                    Email = "DiscountProductsAPI@gmail.com",
                    Url = new Uri("https://SomeRandomUrl/RandomPath"),
                }
            };
        }
    }
}
