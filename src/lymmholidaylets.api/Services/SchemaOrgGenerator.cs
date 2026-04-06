using LymmHolidayLets.Application.Model.Property;

namespace LymmHolidayLets.Api.Services;

public interface ISchemaOrgGenerator
{
    object Generate(PropertyDetailResult detail, string propertyUrl);
}

public sealed class SchemaOrgGenerator(IImageUrlResolver imageUrlResolver) : ISchemaOrgGenerator
{
    public object Generate(PropertyDetailResult detail, string propertyUrl)
    {
        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"]    = "LodgingBusiness",
            ["name"]     = detail.DisplayAddress,
            ["url"]      = propertyUrl,
            ["description"]   = detail.Description,
            ["numberOfRooms"] = detail.NumberOfBedrooms
        };

        if (detail.Map is not null)
        {
            schema["geo"] = new Dictionary<string, object>
            {
                ["@type"]     = "GeoCoordinates",
                ["latitude"]  = detail.Map.Latitude,
                ["longitude"] = detail.Map.Longitude
            };
        }

        if (detail.RatingSummary is not null)
        {
            schema["aggregateRating"] = new Dictionary<string, object>
            {
                ["@type"]       = "AggregateRating",
                ["ratingValue"] = Math.Round(detail.RatingSummary.Rating, 2),
                ["bestRating"]  = 5,
                ["worstRating"] = 1,
                ["reviewCount"] = detail.RatingSummary.TotalReviews
            };
        }

        if (detail.Amenities.Count > 0)
        {
            schema["amenityFeature"] = detail.Amenities
                .Select(a => new Dictionary<string, object>
                {
                    ["@type"] = "LocationFeatureSpecification",
                    ["name"]  = a,
                    ["value"] = true
                })
                .ToList();
        }

        if (detail.Images.Count > 0)
        {
            schema["image"] = detail.Images
                .Select(i => imageUrlResolver.Resolve(i.ImagePath))
                .Where(url => url is not null)
                .ToList();
        }

        if (detail.CheckInTime != default)
            schema["checkinTime"] = detail.CheckInTime.ToString("HH:mm");

        if (detail.CheckOutTime != default)
            schema["checkoutTime"] = detail.CheckOutTime.ToString("HH:mm");

        return schema;
    }
}
