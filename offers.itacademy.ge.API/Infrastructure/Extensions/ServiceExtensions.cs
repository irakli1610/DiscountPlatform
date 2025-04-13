using offers.itacademy.ge.Application.Interfaces;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Repositories;
using offers.itacademy.ge.Application.Services;
using offers.itacademy.ge.Application.Services.ImageServices;
using offers.itacademy.ge.Infrastructure;
using offers.itacademy.ge.Infrastructure.Categories;
using offers.itacademy.ge.Infrastructure.ProductOffers;
using offers.itacademy.ge.Infrastructure.Purchases;
using offers.itacademy.ge.Infrastructure.Users;

namespace offers.itacademy.ge.API.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            
            services.AddScoped<IUserService,UserService>();
            services.AddScoped<IProductOfferService, ProductOfferService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IFileService, ImageService>();

            AddApplicationRepositories(services);
        }

        private static void AddApplicationRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductOfferRepository, ProductOfferRepository>();
            services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            services.AddScoped<ICategoryRepository,CategoryRepository>();
        }
    }
}
