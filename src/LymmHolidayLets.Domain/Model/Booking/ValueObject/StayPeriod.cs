using System;

namespace LymmHolidayLets.Domain.Model.Booking.ValueObject
{
    public sealed record StayPeriod
    {
        public DateTime CheckIn { get; init; }
        public DateTime CheckOut { get; init; }

        public StayPeriod(DateTime checkIn, DateTime checkOut)
            : this(checkIn, checkOut, allowPastCheckIn: false)
        {
        }

        private StayPeriod(DateTime checkIn, DateTime checkOut, bool allowPastCheckIn)
        {
            if (checkOut <= checkIn)
                throw new ArgumentException("Check-out date must be after check-in date.");

            if (!allowPastCheckIn && checkIn < DateTime.UtcNow.Date)
                throw new ArgumentException("Check-in date cannot be in the past.");

            CheckIn = checkIn;
            CheckOut = checkOut;
        }

        public static StayPeriod Reconstitute(DateTime checkIn, DateTime checkOut) =>
            new(checkIn, checkOut, allowPastCheckIn: true);

        public int Nights => (CheckOut - CheckIn).Days;

        public bool Overlaps(StayPeriod other)
        {
            return CheckIn < other.CheckOut && other.CheckIn < CheckOut;
        }
    }
}
