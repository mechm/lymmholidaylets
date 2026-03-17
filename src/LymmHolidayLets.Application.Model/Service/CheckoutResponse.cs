namespace LymmHolidayLets.Application.Model.Service
{
    /// <summary>
    /// Represents the outcome of a checkout operation.
    /// Exactly one of <see cref="Result"/> (success) or <see cref="Error"/> (failure) will be set —
    /// the ambiguous states that a raw tuple allows (both null, or both set) are not possible here.
    /// </summary>
    public sealed class CheckoutResponse
    {
        private CheckoutResponse() { }

        /// <summary>True when the checkout session was created successfully.</summary>
        public bool IsSuccess => Error is null;

        /// <summary>User-facing error message. Null on success.</summary>
        public string? Error { get; private init; }

        /// <summary>Session data returned on success. Null on failure.</summary>
        public CheckoutResult? Result { get; private init; }

        /// <summary>Creates a successful response carrying the session result.</summary>
        public static CheckoutResponse Success(CheckoutResult result) =>
            new() { Result = result };

        /// <summary>Creates a failed response carrying a user-facing error message.</summary>
        public static CheckoutResponse Failure(string error) =>
            new() { Error = error };
    }
}
