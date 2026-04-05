namespace LymmHolidayLets.Domain.ReadModel.Property
{
    public sealed class PropertyDetailAggregate
    {
        public PropertyDetailAggregate(
            PropertyBooking propertyBooking, 
            IEnumerable<DateOnly> datesBooked, 
            IEnumerable<FAQ> faqs, 
            IEnumerable<Review> review, 
            IEnumerable<string> amenities,
            IEnumerable<PropertyImage> images,
            IEnumerable<PropertyBedroom> bedrooms)
        {
            PropertyBooking = propertyBooking;
            DatesBooked = datesBooked;
            FAQs = faqs;
            Review = review;
            Amenities = amenities;
            Images = images;
            Bedrooms = bedrooms;
        }

        public PropertyBooking PropertyBooking { get; set; }
        public IEnumerable<DateOnly> DatesBooked { get; set; }
        public IEnumerable<FAQ> FAQs { get; set; }
        public IEnumerable<Review> Review { get; set; }
        public IEnumerable<string> Amenities { get; set; }
        public IEnumerable<PropertyImage> Images { get; set; }
        public IEnumerable<PropertyBedroom> Bedrooms { get; set; }
    }
}
