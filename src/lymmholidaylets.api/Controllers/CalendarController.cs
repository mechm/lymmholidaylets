using LymmHolidayLets.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class CalendarController(
        ICalService icalService,
        ILogger<CalendarController> logger) : ControllerBase
    {
        /// <summary>
        /// Returns an iCal (.ics) file containing availability for a specific property.
        /// </summary>
        /// <param name="propertyId">The unique ID of the property (1–255). Example: <c>1</c></param>
        /// <param name="identifier">The access identifier GUID required for authorisation. Example: <c>3fa85f64-5717-4562-b3fc-2c963f66afa6</c></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A downloadable .ics file for calendar synchronisation.</returns>
        /// <response code="200">Returns the iCal file.</response>
        /// <response code="400">Invalid property ID or matching calendar not found.</response>
        /// <response code="500">Internal server error during calendar generation.</response>
        [HttpGet("ical/{propertyId:int:min(1):max(255)}.ics")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ICal(byte propertyId, [FromQuery] Guid identifier, CancellationToken cancellationToken)
        {
            var result = await icalService.GetCalendarAsync(propertyId, identifier, cancellationToken);

            if (result == null)
            {
                logger.LogWarning("Failed to retrieve or generate calendar for PropertyId={PropertyId}, Identifier={Identifier}", propertyId, identifier);
                return BadRequest("No matching calendar found or invalid request.");
            }

            logger.LogInformation("iCal served for PropertyId={PropertyId}", propertyId);
            return result;
        }
    }
}
