using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using offers.itacademy.ge.Application.Interfaces;
using offers.itacademy.ge.Application.Repositories;
using offers.itacademy.ge.ExpirationMonitorWorker.BackgroundWorkers;
using offers.itacademy.ge.Infrastructure;
using offers.itacademy.ge.Infrastructure.Categories;
using offers.itacademy.ge.Infrastructure.ProductOffers;
using offers.itacademy.ge.Infrastructure.Purchases;
using offers.itacademy.ge.Infrastructure.Users;
using offers.itacademy.ge.Persistance.Context;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting ExpirationMonitorWorker");

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseUrls("http://localhost:5001");
            webBuilder.ConfigureServices(services =>
            {
                services.AddControllers();
                services.AddHealthChecks();

                services.AddDbContext<DiscountPlatformContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IProductOfferRepository, ProductOfferRepository>();
                services.AddScoped<IPurchaseRepository, PurchaseRepository>();
                services.AddScoped<ICategoryRepository, CategoryRepository>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();

                services.AddHostedService<OfferExpirationWorker>();
            });
            webBuilder.Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/worker-health", () => Microsoft.AspNetCore.Http.Results.Ok("Worker is alive"));
                });
            });
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ExpirationMonitorWorker failed to start");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}