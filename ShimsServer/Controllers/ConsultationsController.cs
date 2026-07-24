using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ShimsServer.Models.ConsultingRoom;
using ShimsServer.Models.Drugs;
using ShimsServer.Models.Investigations;
using ShimsServer.Models.Services;
using ShimsServer.Repositories;

namespace ShimsServer.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ConsultationsController(
        IConsultationsRepository dataSource,
        ILogger<ConsultationsController> logger) : ControllerBase
    {
        [HttpGet("{id:required:guid}")]
        [ProducesResponseType(typeof(IEnumerable<PatientConsultationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<PatientConsultationDto>> GetConsultations(Guid id) => await dataSource.GetConsultations(id, HttpContext.RequestAborted);


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddConsultation([FromBody] AddPatientConsultationDto dto)
        {
            (Guid id, string user) info = (Guid.NewGuid(), User.Identity?.Name ?? "UnknownUser");

            try
            {
                var result = await dataSource.AddConsultation(dto, info, HttpContext.RequestAborted);
                if (result != 1)
                    return BadRequest(new { message = "No consultation was added. Please check the input and try again." });
                return Ok(new { message = "Consultation added successfully." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error adding consultation");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding consultation");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during consultation addition." });
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteConsultation(Guid id)
        {
            try
            {
                var result = await dataSource.DeleteConsultation(id, HttpContext.RequestAborted);
                if (result != 1)
                    return BadRequest(new { message = "No consultation was deleted. Please verify the ID and try again." });
                return Ok(new { message = "Consultation deleted successfully." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error deleting consultation {ConsultationId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting consultation {ConsultationId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during consultation deletion." });
            }
        }

        #region lab-requests

        [HttpPost("lab-request")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddLabRequest([FromBody] AddInvestigationRequestDto request)
        {
            (Guid id, string user) info = (Guid.CreateVersion7(), User.Identity?.Name ?? "UnknownUser");

            try
            {
                var result = await dataSource.AddInvestigationRequest(request, info, HttpContext.RequestAborted);
                if (result != 1)
                    return BadRequest(new { message = "No lab request was added. Please check the input and try again." });
                return Ok(new { message = "Lab request added successfully." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error adding lab request");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding lab request");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during lab request addition." });
            }
        }

        [HttpDelete("lab-request/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteLabRequest(Guid id)
        {
            try
            {
                var result = await dataSource.DeleteRequest(id, HttpContext.RequestAborted);
                if (result != 1)
                    return BadRequest(new { message = "No lab request was deleted. Please verify the ID and try again." });
                return Ok(new { message = "Lab request deleted successfully." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error deleting lab request {LabRequestId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting lab request {LabRequestId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during lab request deletion." });
            }
        }

        #endregion

        #region prescriptions

        /// <summary>
        /// Phase 1: Physician makes a drug prescription with multiple drugs
        /// </summary>
        [HttpPost("prescription")]
        [ProducesResponseType(typeof(PrescriptionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddPrescription([FromBody] AddDrugRequestDto request)
        {
            // Explicit validation for empty array (ModelState validation is automatic in [ApiController])
            if (request.Drugs?.Length < 1)
                return BadRequest(new { message = "At least one drug is required in a prescription." });

            (Guid id, string user) info = (Guid.CreateVersion7(), User.Identity?.Name ?? "UnknownUser");

            try
            {
                var result = await dataSource.AddPrescription(request, info, HttpContext.RequestAborted);
                if (result < 1)
                    return BadRequest(new { message = "No prescription was added. Please check the input and try again." });
                return Ok(info.id);
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error adding prescription");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding prescription");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during prescription addition." });
            }
        }

        /// <summary>
        /// Get a prescription with all its drugs
        /// </summary>
        [HttpGet("prescription/{prescriptionId:guid}")]
        [ProducesResponseType(typeof(PrescriptionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PrescriptionResponseDto>> GetPrescription(Guid prescriptionId)
        {
            try
            {
                // TODO: Implement get prescription from repository
                return StatusCode(StatusCodes.Status501NotImplemented, new { message = "Get prescription endpoint not yet implemented." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error retrieving prescription {PrescriptionId}", prescriptionId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving prescription {PrescriptionId}", prescriptionId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the prescription." });
            }
        }

        [HttpPut("prescription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdatePrescription([FromBody] EditDrugsRequestDto request)
        {
            (Guid id, string user) info = (request.DrugsRequestDetailsID, User.Identity?.Name ?? "UnknownUser");

            try
            {
                var result = await dataSource.UpdatePrescription(request, info, HttpContext.RequestAborted);
                if (result != 1)
                    return BadRequest(new { message = "No prescription was updated. Please verify the ID and try again." });
                return Ok(new { message = "Prescription updated successfully." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error updating prescription {DrugDetailId}", request.DrugsRequestDetailsID);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating prescription {DrugDetailId}", request.DrugsRequestDetailsID);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during prescription update." });
            }
        }

        [HttpDelete("prescription/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeletePrescription(Guid id)
        {
            try
            {
                var result = await dataSource.DeletePrescription(id, HttpContext.RequestAborted);
                if (result != 1)
                    return BadRequest(new { message = "No prescription was deleted. Please verify the ID and try again." });
                return Ok(new { message = "Prescription deleted successfully." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error deleting prescription {PrescriptionId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting prescription {PrescriptionId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during prescription deletion." });
            }
        }

        #endregion

        #region services-requests

        [HttpPost("service-request")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddServiceRequest([FromBody] AddServiceRequestDto request)
        {
            (Guid id, string user) info = (Guid.CreateVersion7(), User.Identity?.Name ?? "UnknownUser");

            try
            {
                var result = await dataSource.AddServvicesRequest(request, info, HttpContext.RequestAborted);
                if (result != 1)
                    return BadRequest(new { message = "No service request was added. Please check the input and try again." });
                return Ok(new { message = "Service request added successfully." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error adding service request");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding service request");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during service request addition." });
            }
        }

        [HttpDelete("service-request/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteServiceRequest(Guid id)
        {
            try
            {
                var result = await dataSource.DeleteServvicesRequest(id, HttpContext.RequestAborted);
                if (result != 1)
                    return BadRequest(new { message = "No service request was deleted. Please verify the ID and try again." });
                return Ok(new { message = "Service request deleted successfully." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error deleting service request {ServiceRequestId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting service request {ServiceRequestId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during service request deletion." });
            }
        }

        #endregion

        #region outcomes

        [HttpPost("outcome")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddOutcome([FromBody] AddPatientOutcomeDto dto)
        {
            (Guid id, string user) info = (Guid.CreateVersion7(), User.Identity?.Name ?? "UnknownUser");

            try
            {
                var result = await dataSource.AddOutcome(dto, info, HttpContext.RequestAborted);
                if (result != 1)
                    return BadRequest(new { message = "No outcome was added. Please check the input and try again." });
                return Ok(new { message = "Outcome added successfully." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error adding outcome");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding outcome");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during outcome addition." });
            }
        }

        [HttpDelete("outcome/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteOutcome(Guid id)
        {
            try
            {
                var result = await dataSource.DeleteOutcome(id, HttpContext.RequestAborted);
                if (result != 1)
                    return BadRequest(new { message = "No outcome was deleted. Please verify the ID and try again." });
                return Ok(new { message = "Outcome deleted successfully." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error deleting outcome {OutcomeId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting outcome {OutcomeId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during outcome deletion." });
            }
        }

        #endregion
    }
}