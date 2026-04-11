using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Application.Interface.Service;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    /// <summary>
    /// Provides iCal feed endpoints for property availability synchronisation.
    /// Calendars are cached until the next midnight UTC and invalidated immediately on new bookings.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class CalendarController(
        ICalendarFeedService calendarFeedService,
        ILogger<CalendarController> logger) : ControllerBase
    {
        /// <summary>
        /// Returns an iCal (.ics) file containing availability blocks for a specific property.
        /// The feed is compatible with Google Calendar, Apple Calendar, and similar clients.
        /// Results are cached until the next midnight UTC; the cache is invalidated immediately
        /// when a new booking is confirmed so availability is always accurate.
        /// </summary>
        /// <param name="propertyId">
        /// The unique numeric identifier for the property (1–255). Example: <c>1</c>
        /// </param>
        /// <param name="identifier">
        /// A per-property GUID that acts as a secret token to authorise calendar access.
        /// Must match the identifier stored against the property in the database.
        /// Example: <c>3fa85f64-5717-4562-b3fc-2c963f66afa6</c>
        /// </param>
        /// <param name="cancellationToken">
        /// Propagates notification that the request has been cancelled by the caller.
        /// </param>
        /// <returns>
        /// A <c>text/calendar</c> (.ics) file attachment suitable for direct import or subscription.
        /// </returns>
        /// <response code="200">Returns the iCal (.ics) file for the requested property.</response>
        /// <response code="400">
        /// The property ID was invalid, the identifier did not match, or no calendar data was found.
        /// </response>
        /// <response code="500">An unexpected error occurred while generating the calendar.</response>
        [HttpGet("ical/{propertyId:int:min(1):max(255)}.ics")]
        [Produces("text/calendar")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ICal(byte propertyId, [FromQuery] Guid identifier, CancellationToken cancellationToken)
        {
            var result = await calendarFeedService.GetCalendarAsync(propertyId, identifier, cancellationToken);

            if (result is null)
            {
                logger.LogWarning("Failed to retrieve or generate calendar for PropertyId={PropertyId}", propertyId);
                return BadRequest(ApiResponse<object>.FailureResult("No matching calendar found or invalid request."));
            }

            logger.LogInformation("iCal served for PropertyId={PropertyId}", propertyId);
            return new FileContentResult(result.FileContents, result.ContentType)
            {
                FileDownloadName = result.FileDownloadName
            };
        }
    }
}
