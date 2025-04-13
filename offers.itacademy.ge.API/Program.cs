using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using offers.itacademy.ge.API.Infrastructure.Extensions;
using offers.itacademy.ge.API.Infrastructure.Middlewares;
using offers.itacademy.ge.Application.Extensions;
using offers.itacademy.ge.Persistance.Connection;
using offers.itacademy.ge.Persistance.Context;
using offers.itacademy.ge.Persistance.Seed;
using Serilog;
using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using offers.itacademy.ge.Application.Services.ImageServices;
using offers.itacademy.ge.Application.Utils.Auth.JWT;
using Microsoft.Extensions.Options;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocs();
builder.Services.AddServices();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

builder.Host.UseSerilog();

builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection(nameof(ConnectionStrings)));
builder.Services.AddDbContext<DiscountPlatformContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(ConnectionStrings.DefaultConnection))));

builder.Services.AddHttpContextAccessor();

var applicationAssembly = Assembly.Load("offers.itacademy.ge.Application");
builder.Services.AddValidatorsFromAssembly(applicationAssembly);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.Configure<JWTConfiguration>(builder.Configuration.GetSection(nameof(JWTConfiguration)));
builder.Services.Configure<ImageUploadSettings>(builder.Configuration.GetSection(nameof(ImageUploadSettings)));
builder.Services.AddTokenAuthentication(builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JWTConfiguration>>());

builder.Services.AddApiVersioningSupport();

builder.Services.AddhealthCheckExtension(builder.Configuration.GetConnectionString(nameof(ConnectionStrings.DefaultConnection)));

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Discount products Management API v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Discount products Management API v2");
    });
    DiscountPlatformSeed.Initialize(app.Services);
}
app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider("C:/Users/irakl/Desktop/TBC .Net Course/Final Project/SharedFiles/uploads"),
    RequestPath = "/uploads"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/api/ping", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("api")
});

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
});

app.MapControllers();

app.Run();
