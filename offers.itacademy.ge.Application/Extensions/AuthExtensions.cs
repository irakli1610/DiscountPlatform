using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using offers.itacademy.ge.Application.Utils.Auth.JWT;
using System.Text;

namespace offers.itacademy.ge.Application.Extensions
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IOptions<JWTConfiguration> jwtOptions)
        {
            var bytes = Encoding.ASCII.GetBytes(jwtOptions.Value.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x => x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(bytes),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = jwtOptions.Value.Audience,
                    ValidIssuer = jwtOptions.Value.Issuer

                });
            return services;

        }    
    }
}
