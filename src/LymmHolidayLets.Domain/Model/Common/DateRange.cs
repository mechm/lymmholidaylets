namespace LymmHolidayLets.Domain.Model.Common
{
    public sealed record DateRange
    {
        public DateOnly CheckIn { get; }
        public DateOnly CheckOut { get; }

        public DateRange(DateOnly checkIn, DateOnly checkOut)
        {
            if (checkOut <= checkIn)
            {
                throw new ArgumentException("Check-out date must be after the check-in date.", nameof(checkOut));
            }

            CheckIn = checkIn;
            CheckOut = checkOut;
        }

        public int Nights => CheckOut.DayNumber - CheckIn.DayNumber;

        public bool Overlaps(DateRange other)
        {
            return CheckIn < other.CheckOut && other.CheckIn < CheckOut;
        }

        public override string ToString() => $"{CheckIn:dd/MM/yyyy} to {CheckOut:dd/MM/yyyy}";
    }
}
