namespace offers.itacademy.ge.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public string Code { get; } = "Entity Not Found";

        public NotFoundException(string message) : base(message)
        {
        }
    }
}
