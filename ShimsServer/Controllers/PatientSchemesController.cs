using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ShimsServer.Models.Records;
using ShimsServer.Repositories;

namespace ShimsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PatientSchemesController(IPatientSchemesRepository repository, ILogger<PatientSchemesController> logger) : ControllerBase
    {

        /// <summary>
        /// Add a new patient insurance scheme
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPatientScheme([FromBody] AddPatientSchemeDto schemeDto)
        {
            try
            {
                var id = Guid.CreateVersion7();
                var res = await repository.AddPatientScheme(schemeDto, (id, User.Identity!.Name!), HttpContext.RequestAborted);

                return res == 1 ? Ok(id) : BadRequest(new { message = "Failed to add patient insurance scheme." });
            }
            catch (PostgresException pex)
            {
                logger.LogError(pex.Message, "Error adding patient insurance scheme");
                return BadRequest(new { Message = "An error occurred while adding the patient insurance scheme." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding patient insurance scheme");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while adding the patient insurance scheme." });
            }
        }

        /// <summary>
        /// Update a patient insurance scheme   
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditPatientScheme(Guid id,[FromBody] EditPatientSchemeDto scheme)
        {
            try
            {
                if (!await repository.PatientHasScheme(scheme.PatientsID, scheme.PatientSchemesID, HttpContext.RequestAborted))
                    return BadRequest(new { Message = "Patient insurance scheme not found" });
                var result = await repository.EditPatientScheme(scheme, User.Identity!.Name!, HttpContext.RequestAborted);

                return result == 1 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError, new { message = "Patient insurance scheme not found." });
            }
            catch (PostgresException pex)
            {
                logger.LogError(pex, "Error updating patient insurance scheme");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the patient insurance scheme." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating patient insurance scheme");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the patient insurance scheme." });
            }
        }

        /// <summary>
        /// Delete a patient insurance scheme       
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePatientScheme(Guid id)
        {
            try
            {
                if (!await repository.PatientSchemeExists(id, HttpContext.RequestAborted))
                    return BadRequest(new { Message = "Patient insurance scheme not found" });
                var n = await repository.DeletePatientScheme(id, HttpContext.RequestAborted);
                return n == 1 ?
                    Ok():
                BadRequest(new { message = "Patient insurance scheme not found." });
            }
            catch (PostgresException pex)
            {
                logger.LogError(pex, "Error deleting patient insurance scheme");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the patient insurance scheme." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting patient insurance scheme");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the patient insurance scheme." });
            }
        }
    }
}
