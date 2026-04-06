namespace LymmHolidayLets.Domain.ReadModel.Property
{
    public sealed class PropertyDetailAggregate(
        PropertyBooking propertyBooking,
        IEnumerable<DateOnly> datesBooked,
        IEnumerable<FAQ> faqs,
        IEnumerable<Review> review,
        IEnumerable<string> amenities,
        IEnumerable<PropertyImage> images,
        IEnumerable<PropertyBedroom> bedrooms)
    {
        public PropertyBooking PropertyBooking { get; set; } = propertyBooking;
        public IEnumerable<DateOnly> DatesBooked { get; set; } = datesBooked;
        public IEnumerable<FAQ> FAQs { get; set; } = faqs;
        public IEnumerable<Review> Review { get; set; } = review;
        public IEnumerable<string> Amenities { get; set; } = amenities;
        public IEnumerable<PropertyImage> Images { get; set; } = images;
        public IEnumerable<PropertyBedroom> Bedrooms { get; set; } = bedrooms;
    }
}
