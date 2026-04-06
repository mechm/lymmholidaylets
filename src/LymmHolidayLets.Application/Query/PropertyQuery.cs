using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Model.Property;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Property;
using LymmHolidayLets.Domain.Repository.EF;
using LymmHolidayLets.Domain.Model.Property.Entity;

namespace LymmHolidayLets.Application.Query
{
    public sealed class PropertyQuery(
        IDapperPropertyDataAdapter propertyDataAdapter,
        IPropertyRepositoryEF propertyRepositoryEf)
        : IPropertyQuery
    {
        public PropertyBooking GetPropertyBookingById(byte propertyId)
        {
            return propertyDataAdapter.GetPropertyBookingById(propertyId);
        }

        public PropertyDetailAggregate? GetPropertyDetailById(byte propertyId) 
        {
            return propertyDataAdapter.GetPropertyDetailById(propertyId);
        }

        public PropertyCheckInCheckOutTime? GetPropertyCheckInCheckOutTime(byte propertyId)
        {
            return propertyDataAdapter.GetPropertyCheckInCheckOutTime(propertyId);
        }

        public async Task<DateTime?> GetCalendarLastModifiedAsync(byte propertyId)
        {
            return await propertyDataAdapter.GetCalendarLastModifiedAsync(propertyId);
        }

        // EF-based surface for GraphQL

        public IQueryable<PropertyEF> GetPropertyByIdEf(byte id)
        {
            return propertyRepositoryEf.GetPropertyById(id);
        }

        public async Task<PropertyDetailResult?> GetPropertyDetailByIdAsync(byte propertyId)
        {
            var aggregate = await propertyDataAdapter.GetPropertyDetailByIdAsync(propertyId);

            if (aggregate is null)
                return null;

            var reviews = aggregate.Review.ToList();
            var totalReviewCount = reviews.FirstOrDefault()?.TotalReviewCount ?? 0;

            PropertyRatingSummaryResult? ratingSummary = reviews.Count > 0
                ? new PropertyRatingSummaryResult
                {
                    Rating        = reviews.Average(r => r.Rating),
                    Accuracy      = NullableAverage(reviews, r => r.Accuracy),
                    Cleanliness   = NullableAverage(reviews, r => r.Cleanliness),
                    Communication = NullableAverage(reviews, r => r.Communication),
                    CheckInExperience = NullableAverage(reviews, r => r.Checkin),
                    Value         = NullableAverage(reviews, r => r.Value),
                    Location      = NullableAverage(reviews, r => r.Location),
                    Facilities    = NullableAverage(reviews, r => r.Facilities),
                    Comfort       = NullableAverage(reviews, r => r.Comfort),
                    TotalReviews  = totalReviewCount
                }
                : null;

            var reviewResults = reviews.Select(r => new PropertyReviewResult
            {
                Name          = r.Name,
                Company       = r.Company,
                Position      = r.Position,
                Description   = r.Description,
                Rating        = r.Rating,
                DateTimeAdded = r.DateTimeAdded,
                ReviewType    = r.ReviewType,
                LinkToView    = r.LinkToView
            }).ToList();

            // Map host information
            PropertyHostResult? host = !string.IsNullOrWhiteSpace(aggregate.PropertyBooking.HostName)
                ? new PropertyHostResult
                {
                    Name               = aggregate.PropertyBooking.HostName,

                    NumberOfProperties = aggregate.PropertyBooking.NumberOfProperties,
                    YearsExperience    = aggregate.PropertyBooking.HostYearsExperience,
                    JobTitle           = aggregate.PropertyBooking.HostJobTitle,
                    ProfileBio         = aggregate.PropertyBooking.HostProfileBio,
                    Location           = aggregate.PropertyBooking.HostLocation,
                    ImagePath          = aggregate.PropertyBooking.HostImagePath
                }
                : null;

            // Map map information
            PropertyMapResult? map = aggregate.PropertyBooking.ShowMap
                ? new PropertyMapResult
                {
                    ShowStreetView       = aggregate.PropertyBooking.ShowStreetView,
                    Latitude             = aggregate.PropertyBooking.Latitude,
                    Longitude            = aggregate.PropertyBooking.Longitude,
                    MapZoom              = aggregate.PropertyBooking.MapZoom,
                    StreetViewLatitude   = aggregate.PropertyBooking.StreetViewLatitude,
                    StreetViewLongitude  = aggregate.PropertyBooking.StreetViewLongitude,
                    Pitch                = aggregate.PropertyBooking.Pitch,
                    Yaw                  = aggregate.PropertyBooking.Yaw,
                    Zoom                 = aggregate.PropertyBooking.Zoom
                }
                : null;

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
                RatingSummary           = ratingSummary,
                Reviews                 = reviewResults,
                Host                    = host,
                Map                     = map,
                Amenities               = aggregate.Amenities.ToList(),
                Images                  = aggregate.Images.Select(i => new PropertyImageResult
                                          {
                                              ImagePath     = i.ImagePath,
                                              AltText       = i.AltText,
                                              SequenceOrder = i.SequenceOrder
                                          }).ToList(),
                Bedrooms                = aggregate.Bedrooms.Select(b => new PropertyBedroomResult
                                          {
                                              BedroomNumber = b.BedroomNumber,
                                              BedroomName   = b.BedroomName,
                                              BedType       = b.BedType,
                                              BedTypeIcon   = b.BedTypeIcon,
                                              NumberOfBeds  = b.NumberOfBeds
                                          }).ToList(),
                LastModified            = aggregate.PropertyBooking.Updated.HasValue
                    ? new DateTimeOffset(aggregate.PropertyBooking.Updated.Value, TimeSpan.Zero)
                    : null,
                CalendarLastModified    = aggregate.PropertyBooking.CalendarLastModified,
                VideoHtml               = aggregate.PropertyBooking.VideoHtml,
                Disclaimer              = aggregate.PropertyBooking.Disclaimer,
            };
        }

        /// <summary>
        /// Averages only the non-null values for a sub-rating category.
        /// Returns null when no reviews carry a value for that category.
        /// </summary>
        private static double? NullableAverage(
            IEnumerable<Review> reviews,
            Func<Review, byte?> selector)
        {
            var values = reviews
                .Select(selector)
                .Where(v => v.HasValue)
                .Select(v => (double)v!.Value)
                .ToList();

            return values.Count > 0 ? values.Average() : null;
        }
    }
}
