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
    public class SchemeInvestigationsController(
        ISchemeInvestigationRepository repository,
        ILogger<SchemeInvestigationsController> logger) : ControllerBase
    {
        /// <summary>
        /// Get investigations for a specific scheme
        /// </summary>
        [HttpGet("scheme/{id:guid}")]
        [ProducesResponseType(typeof(IEnumerable<SchemeInvestigationDTO>), StatusCodes.Status200OK)]
        [ResponseCache(Duration = 8640 * 20, Location = ResponseCacheLocation.Client, VaryByQueryKeys = ["id"])]
        public async Task<IEnumerable<SchemeInvestigationDTO>> GetInvestigationsByScheme(Guid id)
        {
            return await repository.GetInvestigationsBySchemeAsync(id, HttpContext.RequestAborted);
        }

        /// <summary>
        /// Create a new scheme investigation pricing
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> AddSchemeInvestigation([FromBody] AddSchemeInvestigationDto dto)
        {
            try
            {
                var userName = User.Identity?.Name ?? "system";
                var siId = await repository.AddSchemeInvestigationAsync(dto, userName, HttpContext.RequestAborted);
                return CreatedAtAction(nameof(GetInvestigationsByScheme), new { id = dto.SchemesID }, siId);
            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "Database error occurred while adding scheme investigation pricing");
                return BadRequest(new { message = "A database error occurred." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while adding scheme investigation pricing");
                return BadRequest(new { message = "An unexpected error occurred." });
            }
        }
    }
}
