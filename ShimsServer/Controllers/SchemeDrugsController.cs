using Microsoft.AspNetCore.Mvc;
using ShimsServer.Data.Repositories;
using ShimsServer.Models.Schemes;
using Microsoft.AspNetCore.Authorization;
using Npgsql;

namespace ShimsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    //[Authorize(Policy = "SysAdmin")]
    public class SchemeDrugsController(ISchemeDrugsRepository repository, ILogger<SchemeDrugsController> logger, CancellationToken token) : ControllerBase
    {

        /// <summary>
        /// Get drugs for a specific scheme
        /// </summary>
        [HttpGet("scheme/{id:guid}")]
        [ProducesResponseType(typeof(IEnumerable<SchemeDrugDTO>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<SchemeDrugDTO>> GetDrugsByScheme(Guid id)
        {
            return await repository.GetDrugsBySchemeAsync(id, token);
        }

        /// <summary>
        /// Create a new scheme drug pricing.
        /// New entries automatically set previous entries to false so that the current entry can be used for billing
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> AddSchemeDrug([FromBody] AddSchemeDrugDto schemeDrugDto)
        {

            try
            {
                var schemeDrugId = await repository.AddSchemeDrugAsync(
                    schemeDrugDto.SchemesID,
                    schemeDrugDto.DrugsID,
                    schemeDrugDto.Price,
                    User.Identity?.Name ?? "System",
                    token);

                return Ok(schemeDrugId);
            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "Database error occurred while adding scheme drug pricing");
                return BadRequest(new { message = "A database error occurred." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while adding scheme drug pricing");
                return BadRequest(new { message = "An unexpected error occurred." });
            }
        }
    }
}

