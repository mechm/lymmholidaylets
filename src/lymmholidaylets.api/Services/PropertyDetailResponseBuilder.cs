using LymmHolidayLets.Api.Models.Property;
using LymmHolidayLets.Application.Model.Property;

namespace LymmHolidayLets.Api.Services;

public sealed class PropertyDetailResponseBuilder(
    ISocialShareLinkGenerator socialShareLinkGenerator,
    ISeoMetaGenerator seoMetaGenerator,
    ISchemaOrgGenerator schemaOrgGenerator,
    IImageUrlResolver imageUrlResolver) : IPropertyDetailResponseBuilder
{
    public PropertyDetailResponse Build(PropertyDetailResult detail)
    {
        var shareLinks = socialShareLinkGenerator.GenerateLinks(detail.PropertyId, detail.DisplayAddress, detail.Slug);

        return new PropertyDetailResponse
        {
            PropertyId              = detail.PropertyId,
            DisplayAddress          = detail.DisplayAddress,
            Description             = detail.Description,
            Slug                    = detail.Slug,
            MinimumNumberOfAdult    = detail.MinimumNumberOfAdult,
            MaximumNumberOfGuests   = detail.MaximumNumberOfGuests,
            MaximumNumberOfAdult    = detail.MaximumNumberOfAdult,
            MaximumNumberOfChildren = detail.MaximumNumberOfChildren,
            MaximumNumberOfInfants  = detail.MaximumNumberOfInfants,
            NumberOfBedrooms        = detail.NumberOfBedrooms,
            NumberOfBathrooms       = detail.NumberOfBathrooms,
            NumberOfReceptionRooms  = detail.NumberOfReceptionRooms,
            NumberOfKitchens        = detail.NumberOfKitchens,
            NumberOfCarSpaces       = detail.NumberOfCarSpaces,
            CheckInTime             = detail.CheckInTime,
            CheckOutTime            = detail.CheckOutTime,
            MinimumStayNights       = detail.MinimumStayNights,
            MaximumStayNights       = detail.MaximumStayNights,
            DatesBooked             = detail.DatesBooked,
            Faqs                    = detail.Faqs,
            RatingSummary           = detail.RatingSummary is not null
                ? PropertyRatingSummaryResponse.FromResult(detail.RatingSummary)
                : null,
            Host                    = detail.Host is not null
                ? new PropertyHostResponse
                  {
                      Name               = detail.Host.Name,
                      JobTitle           = detail.Host.JobTitle,
                      NumberOfProperties = detail.Host.NumberOfProperties,
                      YearsExperience    = detail.Host.YearsExperience,
                      ProfileBio         = detail.Host.ProfileBio,
                      Location           = detail.Host.Location,
                      ImagePath          = imageUrlResolver.Resolve(detail.Host.ImagePath)
                  }
                : null,
            Map                     = detail.Map,
            Amenities               = detail.Amenities,
            Images                  = detail.Images
                .Select(i => new PropertyImageResult
                {
                    ImagePath     = imageUrlResolver.Resolve(i.ImagePath) ?? i.ImagePath,
                    AltText       = i.AltText,
                    SequenceOrder = i.SequenceOrder
                })
                .ToList(),
            Bedrooms                = detail.Bedrooms,
            Reviews                 = detail.Reviews
                .Select(ReviewResponse.FromApplicationModel)
                .ToList(),
            ShareLinks              = new PropertyShareLinksResponse
            {
                Facebook = shareLinks.FacebookShareLink,
                Twitter  = shareLinks.TwitterShareLink,
                LinkedIn = shareLinks.LinkedInShareLink,
                Email    = shareLinks.EmailShareLink
            },
            Seo                     = seoMetaGenerator.Generate(detail, shareLinks.PropertyUrl),
            SchemaOrg               = schemaOrgGenerator.Generate(detail, shareLinks.PropertyUrl),
            LastModified            = detail.LastModified,
            VideoHtml               = detail.VideoHtml,
            Disclaimer              = detail.Disclaimer
        };
    }
}
