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
        /// <param name="id">The unique ID of the property.</param>
        /// <param name="s">The session GUID required for access.</param>
        /// <returns>A downloadable .ics file for calendar synchronization.</returns>
        /// <response code="200">Returns the iCal file.</response>
        /// <response code="400">Invalid property ID or matching calendar not found.</response>
        /// <response code="500">Internal server error during calendar generation.</response>
        [HttpGet("ical/{id:int:min(1)}.ics")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ICal(int id, [FromQuery] Guid s)
        {
            var result = await icalService.GetCalendarAsync(id, s);

            if (result == null)
            {
                logger.LogWarning("Failed to retrieve or generate calendar for PropertyId={PropertyId}, Session={Session}", id, s);
                return BadRequest("No matching calendar found or invalid request.");
            }

            logger.LogInformation("iCal served for PropertyId={PropertyId}", id);
            return result;
        }
    }
}
