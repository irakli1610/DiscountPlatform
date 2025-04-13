namespace offers.itacademy.ge.Application.Exceptions
{
    public class FileUploadException : Exception
    {
        public string Code { get; } = "file_upload_error";
        public FileUploadException(string message) : base(message)
        {
        }
    }
}
