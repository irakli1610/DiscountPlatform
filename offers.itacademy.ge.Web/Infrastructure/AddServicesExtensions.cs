using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Interfaces;
using offers.itacademy.ge.Application.Repositories;
using offers.itacademy.ge.Application.Services;
using offers.itacademy.ge.Infrastructure.Categories;
using offers.itacademy.ge.Infrastructure;
using offers.itacademy.ge.Infrastructure.ProductOffers;
using offers.itacademy.ge.Infrastructure.Purchases;
using offers.itacademy.ge.Infrastructure.Users;
using offers.itacademy.ge.Application.Services.ImageServices;

namespace offers.itacademy.ge.Web.Infrastructure
{
    public static class AddServicesExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IProductOfferService, ProductOfferService>();
            services.AddScoped<IProductOfferRepository, ProductOfferRepository>();

            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IPurchaseRepository, PurchaseRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IFileService, ImageService>();

        }
    }
}
