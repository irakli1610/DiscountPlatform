using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using offers.itacademy.ge.Domain.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace offers.itacademy.ge.Application.Utils.Auth.JWT
{
    public static class JwtHelper
    {
        public static string GenerateToken(User user, IOptions<JWTConfiguration> options)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(options.Value.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {

                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("UserId", user.Id.ToString())

                }),
                Expires = DateTime.UtcNow.AddMinutes(options.Value.ExpirationTimeInMinutes),
                Audience = options.Value.Audience,
                Issuer = options.Value.Issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
