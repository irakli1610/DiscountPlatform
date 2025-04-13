namespace offers.itacademy.ge.Application.Exceptions
{
    public class InvalidCancellationRequestException : Exception
    {
        public string Code { get; } = "invalid_cancellation_request";
        public InvalidCancellationRequestException(string message) : base(message)
        {
        }
    }
}
