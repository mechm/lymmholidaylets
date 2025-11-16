using System;

namespace LymmHolidayLets.Application.Validation
{
    public static class DateValidation
    {
        //https://stackoverflow.com/questions/38593636/mvc-validation-updating-another-field
        public static string? ValidForBooking(DateOnly checkIn, DateOnly checkout, IList<Domain.ReadModel.Calendar.Calendar> ourAvailableCalendarDates)
        {
            if (!ourAvailableCalendarDates.Any())
            {
                return "No Available Dates based on your selection";
            }

            // user selected date range
            // if any dates don't exist from our date collection
            if (GetDateRange(checkIn, checkout).Except(ourAvailableCalendarDates.Select(i => i.Date)).Any())
            {
                return "Dates selected are not available";
            }

            Domain.ReadModel.Calendar.Calendar startDateAvailable = ourAvailableCalendarDates.First();

            // if minimum stay is too low
            if (startDateAvailable.MinimumStay > checkout.DayNumber - checkIn.DayNumber)
            {
                return $"Please choose at least the minimum stay of {startDateAvailable.MinimumStay} nights";
            }

            // if maximum stay is too high
            if (startDateAvailable.MaximumStay.HasValue && startDateAvailable.MaximumStay.Value < checkout.DayNumber - checkIn.DayNumber)
            {
                return $"Please choose a maximum stay of {startDateAvailable.MaximumStay.Value} nights";
            }

            return null;
        }

        private static IEnumerable<DateOnly> GetDateRange(DateOnly startDate, DateOnly endDate)
        {
            while (startDate < endDate)
            {
                yield return startDate;
                startDate = startDate.AddDays(1);
            }
        }
    }
}
