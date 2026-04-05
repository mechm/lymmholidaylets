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

            PropertyReviewAggregateResult? reviewAggregate = reviews.Count > 0
                ? new PropertyReviewAggregateResult
                {
                    OverallRating        = reviews.Average(r => r.Rating),
                    OverallAccuracy      = NullableAverage(reviews, r => r.Accuracy),
                    OverallCleanliness   = NullableAverage(reviews, r => r.Cleanliness),
                    OverallCommunication = NullableAverage(reviews, r => r.Communication),
                    OverallCheckIn       = NullableAverage(reviews, r => r.Checkin),
                    OverallValue         = NullableAverage(reviews, r => r.Value),
                    OverallLocation      = NullableAverage(reviews, r => r.Location),
                    OverallFacilities    = NullableAverage(reviews, r => r.Facilities),
                    OverallComfort       = NullableAverage(reviews, r => r.Comfort),
                    Reviews = reviews.Select(r => new PropertyReviewResult
                    {
                        Name          = r.Name,
                        Company       = r.Company,
                        Position      = r.Position,
                        Description   = r.Description,
                        Rating        = r.Rating,
                        DateTimeAdded = r.DateTimeAdded,
                        ReviewType    = r.ReviewType,
                        LinkToView    = r.LinkToView
                    }).ToList()
                }
                : null;

            // Map host information
            PropertyHostResult? host = !string.IsNullOrWhiteSpace(aggregate.PropertyBooking.HostName)
                ? new PropertyHostResult
                {
                    Name               = aggregate.PropertyBooking.HostName,
                    Location           = aggregate.PropertyBooking.HostLocation,
                    NumberOfProperties = aggregate.PropertyBooking.NumberOfProperties,
                    YearsExperience    = aggregate.PropertyBooking.HostYearsExperience,
                    JobTitle           = aggregate.PropertyBooking.HostJobTitle,
                    ProfileBio         = aggregate.PropertyBooking.HostProfileBio,
                    ImagePath          = aggregate.PropertyBooking.HostImagePath
                }
                : null;

            // Map map information
            PropertyMapResult? map = new PropertyMapResult
            {
                ShowMap              = aggregate.PropertyBooking.ShowMap,
                ShowStreetView       = aggregate.PropertyBooking.ShowStreetView,
                Latitude             = aggregate.PropertyBooking.Latitude,
                Longitude            = aggregate.PropertyBooking.Longitude,
                MapZoom              = aggregate.PropertyBooking.MapZoom,
                StreetViewLatitude   = aggregate.PropertyBooking.StreetViewLatitude,
                StreetViewLongitude  = aggregate.PropertyBooking.StreetViewLongitude,
                Pitch                = aggregate.PropertyBooking.Pitch,
                Yaw                  = aggregate.PropertyBooking.Yaw,
                Zoom                 = aggregate.PropertyBooking.Zoom
            };

            return new PropertyDetailResult
            {
                PropertyId              = aggregate.PropertyBooking.ID,
                DisplayAddress          = aggregate.PropertyBooking.DisplayAddress,
                PageDescription         = aggregate.PropertyBooking.PageDescription,
                MinimumNumberOfAdult    = aggregate.PropertyBooking.MinimumNumberOfAdult,
                MaximumNumberOfGuests   = aggregate.PropertyBooking.MaximumNumberOfGuests,
                MaximumNumberOfAdult    = aggregate.PropertyBooking.MaximumNumberOfAdult,
                MaximumNumberOfChildren = aggregate.PropertyBooking.MaximumNumberOfChildren,
                MaximumNumberOfInfants  = aggregate.PropertyBooking.MaximumNumberOfInfants,
                DatesBooked             = aggregate.DatesBooked.ToList(),
                FaQs                    = aggregate.FAQs.Select(f => new PropertyFaqResult
                                          {
                                              Question = f.Question,
                                              Answer   = f.Answer
                                          }).ToList(),
                ReviewAggregate         = reviewAggregate,
                Host                    = host,
                Map                     = map
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
