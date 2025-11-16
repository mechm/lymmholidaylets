namespace LymmHolidayLets.Domain.ReadModel.Property
{
    public sealed class PropertyDetailAggregate
    {
        public PropertyDetailAggregate(PropertyBooking propertyBooking, IEnumerable<DateOnly> datesBooked, IEnumerable<FAQ> faqs, IEnumerable<Review> review)
        {
            PropertyBooking = propertyBooking;
            DatesBooked = datesBooked;
            FAQs = faqs;
            Review = review;
        }

        public PropertyBooking PropertyBooking { get; set; }
        public IEnumerable<DateOnly> DatesBooked { get; set; }
        public IEnumerable<FAQ> FAQs { get; set; }
        public IEnumerable<Review> Review { get; set; }
    }
}
