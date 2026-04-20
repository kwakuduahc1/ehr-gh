using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ShimsServer.Repositories;

namespace ShimsServer.Controllers.Records
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RegistrationsController(IRegistrationRepository repository, ILogger<RegistrationsController> logger, CancellationToken token) : ControllerBase
    {
        /// <summary>
        /// Register a new patient with scheme and initial attendance
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> RegisterPatient([FromBody] AddPatientDto dto)
        {
            try
            {
                var patientId = Guid.CreateVersion7();
                var attendanceId = Guid.CreateVersion7();
                var userName = User.Identity?.Name ?? "system";

                await repository.AddPatientAsync(dto, (patientId, attendanceId, userName), token);

                return Ok(new { message = "Patient registered successfully", patientId });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error registering patient");
                return BadRequest(new { message = "There was a database level error" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error registering patient");
                return BadRequest(new { message = "An error occurred during patient registration" });
            }
        }

        /// <summary>
        /// Get all patients with their latest attendance
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
        public async Task<ActionResult<IEnumerable<ListPatientsDto>>> GetPatients()
        {
            var patients = await repository.GetPatientsAsync(token);
            return Ok(patients);
        }

        /// <summary>
        /// Get a specific patient by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ListPatientsDto>> GetPatientById(Guid id)
        {
            var patient = await repository.GetPatientByIdAsync(id, token);
            return patient == null ? NotFound() : Ok(patient);
        }

        /// <summary>
        /// Search for patients by name, ID, or card ID
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ListPatientsDto>>> SearchPatients([FromQuery] string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return BadRequest(new { message = "Search term cannot be empty" });

            var patients = await repository.SearchPatientsAsync(search.Trim(), token);
            return Ok(patients);
        }

        /// <summary>
        /// Update patient information
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> UpdatePatient(Guid id, [FromBody] EditPatientDto dto)
        {
            // Verify the ID in the route matches the DTO
            if (id != dto.PatientID)
                return BadRequest(new { message = "Patient ID mismatch" });
            try
            {
            // Check if patient exists
            if (!await repository.PatientExists(id, token))
                return NotFound(new { message = "Patient not found" });
                var userName = User.Identity?.Name ?? "system";
                var rowsAffected = await repository.EditPatientAsync(dto, userName, token);

                if (rowsAffected == 0)
                    return NotFound(new { message = "Patient not found" });

                return Ok(new { message = "Patient updated successfully" });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error updating patient {PatientId}", id);
                return BadRequest(new { message = "There was a database level error" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating patient {PatientId}", id);
                return BadRequest(new { message = "An error occurred during patient update" });
            }
        }

        /// <summary>
        /// Delete (soft delete) a patient
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> DeletePatient(Guid id)
        {
            try
            {
            // Check if patient exists
            if (!await repository.PatientExists(id, token))
                return NotFound(new { message = "Patient not found" });

                await repository.DeletePatientAsync(id, token);
                return Ok(new { message = "Patient deleted successfully" });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error deleting patient {PatientId}", id);
                return BadRequest(new { message = "There was a database level error" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting patient {PatientId}", id);
                return BadRequest(new { message = "An error occurred during patient deletion" });
            }
        }
    }
}
