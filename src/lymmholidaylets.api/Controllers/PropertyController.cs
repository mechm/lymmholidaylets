using Asp.Versioning;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Property;
using LymmHolidayLets.Api.Services;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Model.Property;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LymmHolidayLets.Api.Controllers
{
    /// <summary>
    /// Provides REST endpoints for property detail and listing data.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class PropertyController(
        IMemoryCache cache,
        ILogger<PropertyController> logger,
        IPropertyQuery propertyQuery,
        ISocialShareLinkGenerator socialShareLinkGenerator,
        ISeoMetaGenerator seoMetaGenerator,
        ISchemaOrgGenerator schemaOrgGenerator,
        IImageUrlResolver imageUrlResolver) : ControllerBase
    {
        /// <summary>
        /// Gets full detail for a property including booking capacity, booked dates,
        /// FAQs, aggregated guest reviews, host information, map coordinates, and social sharing links.
        /// Results are cached for 1 hour; the cache entry is evicted immediately when a review or FAQ
        /// is created, updated, or deleted via the respective commands.
        /// </summary>
        /// <param name="id">The numeric property identifier (e.g. <c>1</c>).</param>
        /// <returns>
        /// A <see cref="PropertyDetailResponse"/> wrapped in a standard <see cref="ApiResponse{T}"/>.
        /// </returns>
        /// <response code="200">Property detail returned successfully.</response>
        /// <response code="404">No property was found for the given id.</response>
        /// <response code="500">An unexpected error occurred while retrieving the property.</response>
        [HttpGet("detail/{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PropertyDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Detail(byte id)
        {
            var cacheKey = $"property-detail-{id}";

            if (!cache.TryGetValue(cacheKey, out PropertyDetailResult? detail))
            {
                logger.LogInformation(
                    "Property detail cache miss for PropertyId={PropertyId}. Fetching from database.", id);

                detail = await propertyQuery.GetPropertyDetailByIdAsync(id);

                if (detail is not null)
                {
                    cache.Set(cacheKey, detail, new MemoryCacheEntryOptions
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                    });
                }
            }

            if (detail is null)
            {
                logger.LogWarning("Property not found for PropertyId={PropertyId}", id);
                return NotFound();
            }

            if (detail.LastModified.HasValue)
                Response.GetTypedHeaders().LastModified = detail.LastModified.Value;

            // Generate social sharing links (use slug for SEO-friendly URL when available)
            var shareLinks = socialShareLinkGenerator.GenerateLinks(id, detail.DisplayAddress, detail.Slug);

            var response = new PropertyDetailResponse
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
                    ? new PropertyHostResult
                    {
                        Name               = detail.Host.Name,
                        NumberOfProperties = detail.Host.NumberOfProperties,
                        YearsExperience    = detail.Host.YearsExperience,
                        JobTitle           = detail.Host.JobTitle,
                        ProfileBio         = detail.Host.ProfileBio,
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
                ShareLinks = new PropertyShareLinksResponse
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

            logger.LogDebug("Property detail served for PropertyId={PropertyId}", id);
            return Ok(ApiResponse<PropertyDetailResponse>.SuccessResult(response));
        }
    }
}
