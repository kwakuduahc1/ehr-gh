using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ShimsServer.Models.DTOs;
using ShimsServer.Repositories;

namespace ShimsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SchemesController(ISchemesRepository repository, ILogger<SchemesController> logger) : ControllerBase
    {

        /// <summary>
        /// Get all schemes
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]

        public async Task<ActionResult<IEnumerable<SchemesDTO>>> GetSchemes()
        {
            var schemes = await repository.GetAllSchemesAsync(Request.HttpContext.RequestAborted);
            return Ok(schemes);
        }

        /// <summary>
        /// Get a specific scheme by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SchemesDTO>> GetSchemeById(Guid id)
        {
            var scheme = await repository.GetSchemeByIdAsync(id, Request.HttpContext.RequestAborted);
            return scheme == null ? NotFound() : Ok(scheme);
        }

        /// <summary>
        /// Create a new scheme
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Guid>> AddScheme([FromBody] AddSchemeDto schemeDto)
        {
            if (await repository.SchemeExistsByNameAsync(schemeDto.SchemeName, Request.HttpContext.RequestAborted))
                return Conflict(new { message = $"Scheme with name {schemeDto.SchemeName} already exists." });

            try
            {
                var schemeId = await repository.AddSchemeAsync(schemeDto, Request.HttpContext.RequestAborted);
                return Ok(schemeId);
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Error inserting scheme {SchemeName}", schemeDto.SchemeName);
                return BadRequest(new { message = "There was a database level error" });
            }
        }

        /// <summary>
        /// Update an existing scheme
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateScheme(UpdateSchemeDto schemeDto)
        {

            try
            {
                var check = await repository.GetSchemeByIdAsync(schemeDto.SchemesID, Request.HttpContext.RequestAborted);
                if (check == null)
                    return NotFound();

                var updated = await repository.UpdateSchemeAsync(schemeDto, Request.HttpContext.RequestAborted);
                if (!updated)
                    return BadRequest(new { Message = $"Scheme {schemeDto.SchemeName} does not exist" });

                return Accepted();
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Error updating scheme {SchemeName}", schemeDto.SchemeName);
                return BadRequest(new { message = "There was a database level error" });
            }
        }

        /// <summary>
        /// Delete a scheme by ID
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteScheme(Guid id)
        {
            try
            {
                var deleted = await repository.DeleteSchemeAsync(id, Request.HttpContext.RequestAborted);
                if (!deleted)
                    return NotFound();

                return Ok();
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Error deleting scheme with ID {SchemeID}", id);
                return BadRequest(new { message = "There was a database level error" });
            }
        }
    }
}
