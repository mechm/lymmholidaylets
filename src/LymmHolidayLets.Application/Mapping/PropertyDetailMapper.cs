using LymmHolidayLets.Application.Model.Property;
using LymmHolidayLets.Domain.Model.Review.ValueObject;
using LymmHolidayLets.Domain.ReadModel.Property;

namespace LymmHolidayLets.Application.Mapping
{
    public static class PropertyDetailMapper
    {
        public static PropertyDetailResult Map(PropertyDetailAggregate aggregate)
        {
            var reviews = aggregate.Review.ToList();

            return new PropertyDetailResult
            {
                PropertyId              = aggregate.PropertyBooking.ID,
                DisplayAddress          = aggregate.PropertyBooking.DisplayAddress,
                Description             = aggregate.PropertyBooking.Description,
                MetaDescription         = aggregate.PropertyBooking.MetaDescription,
                Slug                    = aggregate.PropertyBooking.Slug,
                MinimumNumberOfAdult    = aggregate.PropertyBooking.MinimumNumberOfAdult,
                MaximumNumberOfGuests   = aggregate.PropertyBooking.MaximumNumberOfGuests,
                MaximumNumberOfAdult    = aggregate.PropertyBooking.MaximumNumberOfAdult,
                MaximumNumberOfChildren = aggregate.PropertyBooking.MaximumNumberOfChildren,
                MaximumNumberOfInfants  = aggregate.PropertyBooking.MaximumNumberOfInfants,
                NumberOfBedrooms        = aggregate.PropertyBooking.NumberOfBedrooms,
                NumberOfBathrooms       = aggregate.PropertyBooking.NumberOfBathrooms,
                NumberOfReceptionRooms  = aggregate.PropertyBooking.NumberOfReceptionRooms,
                NumberOfKitchens        = aggregate.PropertyBooking.NumberOfKitchens,
                NumberOfCarSpaces       = aggregate.PropertyBooking.NumberOfCarSpaces,
                CheckInTime             = aggregate.PropertyBooking.CheckInTimeAfter,
                CheckOutTime            = aggregate.PropertyBooking.CheckOutTimeBefore,
                MinimumStayNights       = aggregate.PropertyBooking.MinimumStayNights,
                MaximumStayNights       = aggregate.PropertyBooking.MaximumStayNights,
                DatesBooked             = aggregate.DatesBooked.ToList(),
                Faqs                    = aggregate.FAQs.Select(f => new PropertyFaqResult
                                          {
                                              Question = f.Question,
                                              Answer   = f.Answer
                                          }).ToList(),
                RatingSummary           = BuildRatingSummary(reviews),
                Reviews                 = reviews.Select(review => new PropertyReviewResult
                                          {
                                              Name          = review.Name,
                                              Company       = review.Company,
                                              Position      = review.Position,
                                              Description   = review.Description,
                                              Rating        = review.Rating,
                                              DateTimeAdded = review.DateTimeAdded,
                                              ReviewType    = review.ReviewType,
                                              LinkToView    = review.LinkToView
                                          }).ToList(),
                Host                    = BuildHost(aggregate.PropertyBooking),
                Map                     = BuildMap(aggregate.PropertyBooking),
                Amenities               = aggregate.Amenities.ToList(),
                Images                  = aggregate.Images.Select(image => new PropertyImageResult
                                          {
                                              ImagePath     = image.ImagePath,
                                              AltText       = image.AltText,
                                              SequenceOrder = image.SequenceOrder
                                          }).ToList(),
                Bedrooms                = aggregate.Bedrooms.Select(bedroom => new PropertyBedroomResult
                                          {
                                              BedroomNumber = bedroom.BedroomNumber,
                                              BedroomName   = bedroom.BedroomName,
                                              BedType       = bedroom.BedType,
                                              BedTypeIcon   = bedroom.BedTypeIcon,
                                              NumberOfBeds  = bedroom.NumberOfBeds
                                          }).ToList(),
                LastModified            = aggregate.PropertyBooking.Updated.HasValue
                    ? new DateTimeOffset(aggregate.PropertyBooking.Updated.Value, TimeSpan.Zero)
                    : null,
                CalendarLastModified    = aggregate.PropertyBooking.CalendarLastModified,
                VideoHtml               = aggregate.PropertyBooking.VideoHtml,
                Disclaimer              = aggregate.PropertyBooking.Disclaimer,
            };
        }

        private static PropertyRatingSummaryResult? BuildRatingSummary(IReadOnlyCollection<Review> reviews)
        {
            var summary = ReviewRatings.Summarize(
                reviews.Select(review => review.Ratings),
                reviews.FirstOrDefault()?.TotalReviewCount ?? 0);

            return summary is null
                ? null
                : new PropertyRatingSummaryResult
                {
                    Rating            = summary.Overall,
                    Accuracy          = summary.Accuracy,
                    Cleanliness       = summary.Cleanliness,
                    Communication     = summary.Communication,
                    CheckInExperience = summary.CheckIn,
                    Value             = summary.Value,
                    Location          = summary.Location,
                    Facilities        = summary.Facilities,
                    Comfort           = summary.Comfort,
                    TotalReviews      = summary.TotalReviews
                };
        }

        private static PropertyHostResult? BuildHost(PropertyBooking booking)
        {
            return !string.IsNullOrWhiteSpace(booking.HostName)
                ? new PropertyHostResult
                {
                    Name               = booking.HostName,
                    NumberOfProperties = booking.NumberOfProperties,
                    YearsExperience    = booking.HostYearsExperience,
                    JobTitle           = booking.HostJobTitle,
                    ProfileBio         = booking.HostProfileBio,
                    Location           = booking.HostLocation,
                    ImagePath          = booking.HostImagePath
                }
                : null;
        }

        private static PropertyMapResult? BuildMap(PropertyBooking booking)
        {
            return booking.ShowMap
                ? new PropertyMapResult
                {
                    ShowStreetView      = booking.ShowStreetView,
                    Latitude            = booking.Latitude,
                    Longitude           = booking.Longitude,
                    MapZoom             = booking.MapZoom,
                    StreetViewLatitude  = booking.StreetViewLatitude,
                    StreetViewLongitude = booking.StreetViewLongitude,
                    Pitch               = booking.Pitch,
                    Yaw                 = booking.Yaw,
                    Zoom                = booking.Zoom
                }
                : null;
        }
    }
}
