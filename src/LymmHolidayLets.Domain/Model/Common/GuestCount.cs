namespace LymmHolidayLets.Domain.Model.Common
{
    /// <summary>
    /// Immutable value object representing the number of guests for a booking.
    /// Guarantees that at least one adult is always present.
    /// </summary>
    public sealed record GuestCount
    {
        public short Adults { get; }
        public short Children { get; }
        public short Infants { get; }

        private GuestCount(short adults, short children, short infants)
        {
            Adults = adults;
            Children = children;
            Infants = infants;
        }

        /// <summary>
        /// Creates a <see cref="GuestCount"/> without throwing.
        /// Returns <c>false</c> when <paramref name="adults"/> is less than 1.
        /// Nullable parameters default to zero — safe for Stripe metadata serialisation.
        /// </summary>
        public static bool TryCreate(short? adults, short? children, short? infants, out GuestCount guests)
        {
            var a = adults ?? 0;
            var c = children ?? 0;
            var i = infants ?? 0;

            if (a < 1)
            {
                guests = null!;
                return false;
            }

            guests = new GuestCount(a, c, i);
            return true;
        }

        public override string ToString() =>
            $"{Adults} adult(s), {Children} child(ren), {Infants} infant(s)";
    }
}
