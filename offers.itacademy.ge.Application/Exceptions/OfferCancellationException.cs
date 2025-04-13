namespace offers.itacademy.ge.Application.Exceptions
{
    public class OfferCancellationException : Exception
    {
        public string Code { get; } = "OfferCancellationError";

        public OfferCancellationException(string message) : base(message)
        {
            
        }
    }
}
