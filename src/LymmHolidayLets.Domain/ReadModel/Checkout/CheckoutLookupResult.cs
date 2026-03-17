namespace LymmHolidayLets.Domain.ReadModel.Checkout
{
    /// <summary>
    /// Discriminated result of looking up checkout data for a property + date range.
    /// Replaces the previous pattern where <c>null</c> meant "property not found" and
    /// a non-null aggregate with <c>TotalNightlyPrice == null</c> meant "dates unavailable".
    /// </summary>
    public abstract class CheckoutLookupResult
    {
        private CheckoutLookupResult() { }

        /// <summary>The property ID does not exist in the Property table.</summary>
        public sealed class PropertyNotFound : CheckoutLookupResult;

        /// <summary>
        /// The property exists but no calendar rows match <c>Available = 1</c>
        /// for the requested date range — already booked or blocked.
        /// </summary>
        public sealed class DatesUnavailable(string propertyName) : CheckoutLookupResult
        {
            public string PropertyName { get; } = propertyName;
        }

        /// <summary>
        /// All data required to proceed with checkout was found.
        /// <see cref="CheckoutAggregate.TotalNightlyPrice"/> is guaranteed non-null.
        /// </summary>
        public sealed class Available(CheckoutAggregate data) : CheckoutLookupResult
        {
            public CheckoutAggregate Data { get; } = data;
        }
    }
}
