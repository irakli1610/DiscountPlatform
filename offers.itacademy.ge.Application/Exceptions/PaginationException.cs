namespace offers.itacademy.ge.Application.Exceptions
{
    public class PaginationException : Exception
    {
        public string Code { get; } = "Pagination_exception";
        public PaginationException(string message) : base(message)
        {
        }   

    }
}
