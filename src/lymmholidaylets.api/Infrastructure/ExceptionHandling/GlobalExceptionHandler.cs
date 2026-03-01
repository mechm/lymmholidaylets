using Microsoft.AspNetCore.Diagnostics;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Application.Model.Exception;
using System.Net;

namespace LymmHolidayLets.Api.Infrastructure.ExceptionHandling
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // 1. Log the full exception details
            logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            // 2. Determine the status code and user-friendly message
            var (statusCode, message) = exception switch
            {
                InvalidCheckoutDataException => (HttpStatusCode.BadRequest, exception.Message),
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid input provided."),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "You are not authorized to perform this action."),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
            };

            // 3. Prepare the standardized response
            var response = ApiResponse<object>.FailureResult(message);

            // 4. Send the response
            httpContext.Response.StatusCode = (int)statusCode;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true; // Indicates the exception has been handled
        }
    }
}
