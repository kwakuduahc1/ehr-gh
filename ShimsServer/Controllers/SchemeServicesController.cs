using Microsoft.AspNetCore.Mvc;
using ShimsServer.Models.Schemes;
using Npgsql;
using Microsoft.AspNetCore.Authorization;
using ShimsServer.Repositories;

namespace ShimsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    //[Authorize(Policy = "SysAdmin")]
    public class SchemeServicesController(
        ISchemeServicePricingRepository repository,
        ILogger<SchemeServicesController> logger) : ControllerBase
    {
        /// <summary>
        /// Get services for a specific scheme
        /// </summary>
        [HttpGet("scheme/{id:guid}")]
        [ProducesResponseType(typeof(IEnumerable<SchemeServiceDTO>), StatusCodes.Status200OK)]
        //[ResponseCache(Duration = 8640 * 20, Location = ResponseCacheLocation.Client, VaryByQueryKeys = ["id"])]
        public async Task<IEnumerable<SchemeServiceDTO>> GetServicesByScheme(Guid id)
        {
            return await repository.GetServicesBySchemeAsync(id, HttpContext.RequestAborted);
        }

        /// <summary>
        /// Create a new scheme service pricing
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> AddSchemeService([FromBody] AddSchemeServiceDto dto)
        {
            try
            {
                var userName = User.Identity?.Name ?? "system";
                var scId = await repository.AddSchemeServiceAsync(dto, userName, HttpContext.RequestAborted);
                return CreatedAtAction(nameof(GetServicesByScheme), new { id = dto.SchemesID }, scId);
            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "Database error occurred while adding scheme service pricing");
                return BadRequest(new { message = "A database error occurred." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while adding scheme service pricing");
                return BadRequest(new { message = "An unexpected error occurred." });
            }
        }
    }
}
