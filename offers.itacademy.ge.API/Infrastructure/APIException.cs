using Microsoft.AspNetCore.Mvc;
using offers.itacademy.ge.Application.Exceptions;
using System.Net;

namespace offers.itacademy.ge.API.Infrastructure
{
    public class APIException : ProblemDetails
    {
        private readonly HttpContext _httpContext;
        private readonly ILogger _logger;

        public string? TraceId
        {
            get => Extensions.TryGetValue("TraceId", out var traceId) ? (string?)traceId : null;
            set => Extensions["TraceId"] = value;
        }

        public string Code { get; set; }
        public LogLevel LogLevel { get; set; }

        public APIException(HttpContext httpContext, Exception exception, ILogger logger)
        {
            _httpContext = httpContext;
            _logger = logger;

            TraceId = httpContext.TraceIdentifier;
            Instance = _httpContext.Request.Path;

            HandleException((dynamic)exception);
        }

        private void HandleException(FileUploadException exception)
        {
            Code = exception.Code; // "file_upload_error"
            Status = (int)HttpStatusCode.BadRequest;
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400";
            Title = "File upload failed.";
            LogLevel = LogLevel.Information;
            Detail = exception.Message;

            _logger.LogInformation("FileUploadException: {Message}", Detail);
        }

        private void HandleException(InvalidCancellationRequestException exception)
        {
            Code = exception.Code;
            Status = (int)HttpStatusCode.BadRequest;
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400";
            Title = "The cancellation request is invalid.";
            LogLevel = LogLevel.Information;
            Detail = exception.Message;

            _logger.LogInformation("InvalidCancellationRequestException: {Message}", Detail);
        }

        private void HandleException(NotFoundException exception)
        {
            Code = exception.Code;
            Status = (int)HttpStatusCode.NotFound;
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404";
            Title = "NotFoundException";
            Detail = exception.Message;
            LogLevel = LogLevel.Information;
            _logger.LogInformation("NotFoundException: {Message}", exception.Message);
        }

        private void HandleException(ObjectAlreadyExistsException exception)
        {
            Code = exception.Code;
            Status = (int)HttpStatusCode.Conflict;
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/409";
            Title = "Conflict: The resource already exists.";
            LogLevel = LogLevel.Warning;
            Detail = exception.Message;

            _logger.LogInformation("ObjectAlreadyExistsException: {Message}", exception.Message);
        }

        private void HandleException(OfferCancellationException exception)
        {
            Code = exception.Code;
            Status = (int)HttpStatusCode.BadRequest;
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400";
            Title = "Offer cancellation failed.";
            LogLevel = LogLevel.Information;
            Detail = exception.Message;

            _logger.LogInformation("OfferCancellationException: {Message}", Detail);
        }

        private void HandleException(PaginationException exception)
        {
            Code = exception.Code; // Fixed: Use exception.Code instead of exception.Message
            Status = (int)HttpStatusCode.BadRequest;
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400";
            Title = "One or more pagination errors occurred.";
            LogLevel = LogLevel.Information;
            Detail = exception.Message;

            _logger.LogInformation("PaginationException: {Message}", Detail);
        }

        private void HandleException(PurchaseException exception)
        {
            Code = exception.Code;
            Status = (int)HttpStatusCode.BadRequest;
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400";
            Title = "Purchase failed";
            LogLevel = LogLevel.Information;
            Detail = exception.Message;

            _logger.LogInformation("PurchaseException: {Message}", Detail);
        }

        private void HandleException(UnauthorizedAccessException exception)
        {
            Code = "UnauthorizedAccess";
            Status = (int)HttpStatusCode.Unauthorized;
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/401";
            Title = "Unauthorized Access";
            LogLevel = LogLevel.Warning;
            Detail = exception.Message;

            _logger.LogInformation("UnauthorizedAccessException: {Message}", exception.Message);
        }

        private void HandleException(ArgumentException exception)
        {
            Code = "InvalidArgument";
            Status = (int)HttpStatusCode.BadRequest;
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400";
            Title = "Bad Request: Invalid argument passed.";
            LogLevel = LogLevel.Information;
            Detail = exception.Message;

            _logger.LogInformation("ArgumentException: {Message}", exception.Message);
        }

        private void HandleException(Exception exception)
        {
            Code = "UnhandledError";
            Status = (int)HttpStatusCode.InternalServerError;
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500";
            Title = "An unexpected error occurred.";
            Detail = exception.Message;
            LogLevel = LogLevel.Error;
            _logger.LogError(exception, "Unhandled exception occurred.");
        }
    }
}
