using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ShimsServer.Models.DTOs;
using ShimsServer.Repositories;

namespace ShimsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class InvestigationsController(
        IInvestigationsRepository repository,
        ILogger<InvestigationsController> logger) : ControllerBase
    {
        /// <summary>
        /// Get all investigations
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InvestigationsDto>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<InvestigationsDto>> GetInvestigations() =>
            await repository.Investigations(HttpContext.RequestAborted);

        /// <summary>
        /// Get an investigation by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(InvestigationsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInvestigation(Guid id)
        {
            var investigation = await repository.GetInvestigation(id, HttpContext.RequestAborted);
            return investigation is null ? NotFound() : Ok(investigation);
        }

        /// <summary>
        /// Check if an investigation exists by name
        /// </summary>
        [HttpGet("check")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> InvestigationExists([FromQuery] string investigation)
        {
            return await repository.InvestigationExists(investigation, HttpContext.RequestAborted)
                ? Ok()
                : NotFound(new { Message = $"{investigation} not found" });
        }

        /// <summary>
        /// Add a new investigation
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddInvestigation([FromBody] AddInvestigationDto investigationDto)
        {
            try
            {
                var id = Guid.CreateVersion7();
                var result = await repository.AddInvestigation(id, investigationDto, HttpContext.RequestAborted);

                return result == 1
                    ? CreatedAtAction(nameof(GetInvestigation), new { id }, id)
                    : BadRequest(new { message = "Failed to add investigation." });
            }
            catch (PostgresException pex)
            {
                logger.LogError(pex, "Error adding investigation");
                return BadRequest(new { Message = "An error occurred while adding the investigation." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding investigation");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while adding the investigation." });
            }
        }

        /// <summary>
        /// Update an investigation
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateInvestigation(Guid id, [FromBody] UpdateInvestigationDto investigation)
        {
            try
            {
                if (id != investigation.InvestigationsID)
                    return BadRequest(new { Message = "Route ID does not match the investigation ID." });

                if (!await repository.InvestigationExists(id, HttpContext.RequestAborted))
                    return NotFound(new { Message = "Investigation not found." });

                var result = await repository.EditInvestigation(investigation, HttpContext.RequestAborted);
                return result == 1 ? Ok() : NotFound(new { Message = "Investigation not found." });
            }
            catch (PostgresException pex)
            {
                logger.LogError(pex, "Error updating investigation");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the investigation." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating investigation");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the investigation." });
            }
        }

        /// <summary>
        /// Delete an investigation
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteInvestigation(Guid id)
        {
            try
            {
                if (!await repository.InvestigationExists(id, HttpContext.RequestAborted))
                    return NotFound(new { Message = "Investigation not found." });

                var result = await repository.DeleteInvestigation(id, HttpContext.RequestAborted);
                return result == 1 ? Ok() : NotFound(new { Message = "Investigation not found." });
            }
            catch (PostgresException pex)
            {
                logger.LogError(pex, "Error deleting investigation");
                if (pex.SqlState == "23503")
                    return Conflict(new { Message = "Investigation is referenced by other records and cannot be deleted." });
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the investigation." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting investigation");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the investigation." });
            }
        }
    }
}
