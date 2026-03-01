namespace LymmHolidayLets.Api.Models
{
    /// <summary>
    /// A standardized envelope for all API responses.
    /// </summary>
    /// <typeparam name="T">The type of the data being returned.</typeparam>
    public sealed class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the request was successful.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// A message providing additional context (e.g., error details or success confirmation).
        /// </summary>
        public string? Message { get; init; }

        /// <summary>
        /// The actual data payload.
        /// </summary>
        public T? Data { get; init; }

        /// <summary>
        /// A list of validation or business rule errors, if any.
        /// </summary>
        public IEnumerable<string>? Errors { get; init; }

        // Helper methods for common response types
        public static ApiResponse<T> SuccessResult(T? data, string? message = null) 
            => new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> FailureResult(string message, IEnumerable<string>? errors = null) 
            => new() { Success = false, Message = message, Errors = errors };
    }
}
