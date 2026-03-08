using System;

namespace LymmHolidayLets.Domain.Model.Booking.ValueObject
{
    public sealed record StayPeriod
    {
        public DateTime CheckIn { get; init; }
        public DateTime CheckOut { get; init; }

        public StayPeriod(DateTime checkIn, DateTime checkOut)
        {
            if (checkOut <= checkIn)
                throw new ArgumentException("Check-out date must be after check-in date.");

            if (checkIn < DateTime.UtcNow.Date)
                throw new ArgumentException("Check-in date cannot be in the past.");

            CheckIn = checkIn;
            CheckOut = checkOut;
        }

        public int Nights => (CheckOut - CheckIn).Days;

        public bool Overlaps(StayPeriod other)
        {
            return CheckIn < other.CheckOut && other.CheckIn < CheckOut;
        }
    }
}
