namespace offers.itacademy.ge.Application.Exceptions
{
    public class PurchaseException : Exception
    {
        public string Code { get; } = "purchase_error";

        public PurchaseException(string message) : base(message)
        {
        }
    }
}
