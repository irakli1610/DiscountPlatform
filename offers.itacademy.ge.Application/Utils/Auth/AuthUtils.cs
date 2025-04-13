using offers.itacademy.ge.Application.Utils.Auth;

namespace offers.itacademy.ge.Application.Utils
{
    public static  class AuthUtils
    {
        public static string GeneratePasswordHash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        public static  bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }

}
