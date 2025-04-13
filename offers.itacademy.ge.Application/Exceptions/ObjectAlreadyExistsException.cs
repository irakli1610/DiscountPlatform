namespace offers.itacademy.ge.Application.Exceptions
{
    public class ObjectAlreadyExistsException : Exception
    {
        public string Code { get; } = "ObjectAlreadyExists";

        public ObjectAlreadyExistsException(string message) : base(message)
        {

        }
    }
}
