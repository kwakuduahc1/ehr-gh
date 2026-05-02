using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ShimsServer.Models.OPD;
using ShimsServer.Repositories;

namespace ShimsServer.Controllers.Vitals
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VitalsController(IVitalsRepository repository, ILogger<VitalsController> logger) : ControllerBase
    {
        /// <summary>
        /// Get vitals for a specific patient
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(IEnumerable<VitalsDTO>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<VitalsDTO>> GetVitalsForPatient(Guid id) => await repository.GetVitalsForPatient(id, HttpContext.RequestAborted);

        /// <summary>
        /// Add vital signs for a patient
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddVitals([FromBody] AddVitalsDto vitalsDto)
        {
            try
            {
                var vitalsId = Guid.CreateVersion7();
                var userName = User.Identity?.Name ?? "system";

                await repository.AddVitals(vitalsDto, vitalsId, userName, HttpContext.RequestAborted);

                return Ok();
            }
            catch (PostgresException pex)
            {
                logger.LogError(pex, "Database error occurred while adding vitals");
                return BadRequest(new { message = "A database error occurred." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding vitals");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
            }
        }
    }
}
