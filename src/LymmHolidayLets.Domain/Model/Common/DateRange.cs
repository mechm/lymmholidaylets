namespace LymmHolidayLets.Domain.Model.Common
{
    public sealed record DateRange
    {
        public DateOnly CheckIn { get; }
        public DateOnly CheckOut { get; }

        private DateRange(DateOnly checkIn, DateOnly checkOut)
        {
            if (checkOut <= checkIn)
            {
                throw new ArgumentException("Check-out date must be after the check-in date.", nameof(checkOut));
            }

            CheckIn = checkIn;
            CheckOut = checkOut;
        }

        /// <summary>
        /// Creates a <see cref="DateRange"/> without throwing.
        /// Returns <c>false</c> when <paramref name="checkOut"/> is on or before <paramref name="checkIn"/>.
        /// </summary>
        public static bool TryCreate(DateOnly checkIn, DateOnly checkOut, out DateRange range)
        {
            if (checkOut <= checkIn)
            {
                range = null!;
                return false;
            }

            range = new DateRange(checkIn, checkOut);
            return true;
        }

        public int Nights => CheckOut.DayNumber - CheckIn.DayNumber;

        public bool Overlaps(DateRange other)
        {
            return CheckIn < other.CheckOut && other.CheckIn < CheckOut;
        }

        public override string ToString() => $"{CheckIn:dd/MM/yyyy} to {CheckOut:dd/MM/yyyy}";
    }
}
