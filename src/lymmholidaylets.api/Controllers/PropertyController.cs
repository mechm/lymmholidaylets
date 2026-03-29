using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace LymmHolidayLets.Api.Controllers
{
    /// <summary>
    /// Provides REST endpoints for property detail and listing data.
    /// </summary>
    /// <remarks>
    /// TODO: Implement property detail and listing endpoints.
    ///       This controller is a stub — see the GraphQL <c>PropertyQuery</c> for the
    ///       current property query implementation via HotChocolate.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class PropertyController : ControllerBase
    {
    }
}
