namespace offers.itacademy.ge.Application.Utils.Auth.JWT
{
    public class JWTConfiguration
    {
        public string Secret { get; set; }
        public int ExpirationTimeInMinutes { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
    }
}
