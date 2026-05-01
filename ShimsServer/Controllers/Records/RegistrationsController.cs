using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ShimsServer.Repositories;
using Asp.Versioning;

namespace ShimsServer.Controllers.Records
{
    /// <summary>
    /// Manages patient registration, retrieval, search, update, and deletion operations.
    /// Handles patient medical records and their associated insurance scheme information.
    /// </summary>
    /// <remarks>
    /// API Version: 1.0
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("Registrations")]
    public class RegistrationsController(IRegistrationRepository repository, ILogger<RegistrationsController> logger) : ControllerBase
    {
        /// <summary>
        /// Retrieves detailed patient information including attendance history and insurance schemes.
        /// </summary>
        /// <param name="id">The unique identifier of the patient</param>
        /// <param name="cancellationToken">Cancellation token for the async operation</param>
        /// <returns>Patient details with attendance and scheme information</returns>
        /// <response code="200">Successfully retrieved patient details</response>
        /// <response code="404">Patient not found</response>
        [HttpGet("details/{id:guid:required}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientDetailsDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientDetailsDto?>> GetPatientDetails(Guid id)
        {
            var patientDetails = await repository.GetPatientDetailsByIdAsync(id, HttpContext.RequestAborted);
            return patientDetails == null ? NotFound() : Ok(patientDetails);
        }

        /// <summary>
        /// Registers a new patient with their scheme and initial attendance record.
        /// </summary>
        /// <param name="dto">Patient registration data containing personal and demographic information</param>
        /// <returns>The newly created patient ID and hospital ID</returns>
        /// <response code="200">Patient successfully registered</response>
        /// <response code="400">Invalid patient data or database error</response>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RegisterPatient([FromBody] AddPatientDto dto)
        {
            List<Guid> psids = [];
            var (pid, attendanceId, userName) = (Guid.CreateVersion7(), Guid.CreateVersion7(), User.Identity?.Name ?? "system");
            try
            {
                var hid = await repository.AddPatientAsync(dto, (pid, attendanceId, psids.ToArray(), userName), HttpContext.RequestAborted);
                return Ok(new { pid, hid });
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
        /// Retrieves all patients with their latest attendance and insurance scheme information.
        /// Results are cached for 30 seconds.
        /// </summary>
        /// <returns>A collection of patient details</returns>
        /// <response code="200">Successfully retrieved all patients</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PatientDetailsDto>))]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
        public async Task<IEnumerable<PatientDetailsDto>> GetPatients()
        {
            return await repository.GetPatientsAsync(HttpContext.RequestAborted);
        }

        /// <summary>
        /// Retrieves a specific patient by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the patient</param>
        /// <returns>Patient details if found</returns>
        /// <response code="200">Successfully retrieved patient information</response>
        /// <response code="404">Patient not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientDetailsDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientDetailsDto>> GetPatientById(Guid id)
        {
            var patient = await repository.GetPatientByIdAsync(id, HttpContext.RequestAborted);
            return patient == null ? NotFound() : Ok(patient);
        }

        /// <summary>
        /// Searches for patients by full name, hospital ID, or insurance card ID.
        /// </summary>
        /// <param name="search">Search term to match against patient name, hospital ID, or card ID</param>
        /// <returns>Collection of matching patients</returns>
        /// <response code="200">Successfully retrieved search results</response>
        /// <response code="400">Search term is empty or invalid</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PatientDetailsDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PatientDetailsDto>>> SearchPatients([FromQuery] string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return BadRequest(new { message = "Search term cannot be empty" });

            var patients = await repository.SearchPatientsAsync(search.Trim(), HttpContext.RequestAborted);
            return Ok(patients);
        }

        /// <summary>
        /// Updates an existing patient's personal and demographic information.
        /// </summary>
        /// <param name="id">The unique identifier of the patient to update</param>
        /// <param name="dto">Updated patient data</param>
        /// <returns>Confirmation message if successful</returns>
        /// <response code="200">Patient successfully updated</response>
        /// <response code="400">Database error</response>
        /// <response code="404">Patient not found</response>
        [HttpPut("{id:guid}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> UpdatePatient(Guid id, [FromBody] EditPatientDto dto)
        {
            try
            {
                // Check if patient exists
                if (!await repository.PatientExists(id, HttpContext.RequestAborted))
                    return NotFound(new { message = "Patient not found" });

                var userName = User.Identity?.Name ?? "system";
                // Route ID is authoritative
                var updatedDto = dto with { PatientsID = id };
                var rowsAffected = await repository.EditPatientAsync(updatedDto, userName, HttpContext.RequestAborted);

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
        /// Performs a soft delete of a patient record, marking it as inactive.
        /// The record is not permanently removed from the database.
        /// </summary>
        /// <param name="id">The unique identifier of the patient to delete</param>
        /// <returns>Confirmation message if successful</returns>
        /// <response code="200">Patient successfully deleted</response>
        /// <response code="404">Patient not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            try
            {
                // Check if patient exists
                if (!await repository.PatientExists(id, HttpContext.RequestAborted))
                    return NotFound(new { message = "Patient not found" });

                await repository.DeletePatientAsync(id, HttpContext.RequestAborted);
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
