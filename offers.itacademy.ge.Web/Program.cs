using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using offers.itacademy.ge.Application.Utils.Auth.JWT;
using offers.itacademy.ge.Persistance.Connection;
using offers.itacademy.ge.Persistance.Context;
using offers.itacademy.ge.Web.Infrastructure;
using System.Reflection;
using offers.itacademy.ge.Application.Services.ImageServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddServices();


builder.Services.Configure<JWTConfiguration>(builder.Configuration.GetSection(nameof(JWTConfiguration)));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Login"; // Redirect to login if unauthenticated
    options.AccessDeniedPath = "/AccessDenied"; // Redirect if unauthorized
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var jwtOptions = builder.Configuration.GetSection("JWTConfiguration").Get<JWTConfiguration>();
    var key = Encoding.ASCII.GetBytes(jwtOptions.Secret);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience
    };
});

builder.Services.AddDbContext<DiscountPlatformContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(ConnectionStrings.DefaultConnection))));


builder.Services.Configure<ImageUploadSettings>(builder.Configuration.GetSection(nameof(ImageUploadSettings)));


var applicationAssembly = Assembly.Load("offers.itacademy.ge.Application");
builder.Services.AddValidatorsFromAssembly(applicationAssembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider("C:/Users/irakl/Desktop/TBC .Net Course/Final Project/SharedFiles/uploads"),
    RequestPath = "/uploads"
});

app.UseRouting();

app.UseAuthentication();    
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=index}/{id?}");

app.MapGet("/ping", () => Results.Ok("MVC is running."));


app.Run();
