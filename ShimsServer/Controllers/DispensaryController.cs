using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ShimsServer.Models.Drugs;
using ShimsServer.Repositories;

namespace ShimsServer.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DispensaryController(
        IDispensaryRepository dispensaryRepository,
        ILogger<DispensaryController> logger) : ControllerBase
    {
        #region calculations

        /// <summary>
        /// Phase 2: Get dispensing calculation details for a drug request
        /// </summary>
        [HttpGet("calculations/{drugsRequestId:guid}")]
        [ProducesResponseType(typeof(DispensingCalculationDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DispensingCalculationDetailDto>> GetCalculation(Guid drugsRequestId)
        {
            try
            {
                // TODO: Implement get calculation from repository
                return StatusCode(StatusCodes.Status501NotImplemented, new { message = "Get calculation endpoint not yet implemented." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error retrieving calculation {DrugsRequestId}", drugsRequestId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving calculation {DrugsRequestId}", drugsRequestId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the calculation." });
            }
        }

        /// <summary>
        /// Phase 2: Pharmacist calculates the quantity to be dispensed
        /// </summary>
        [HttpPost("calculate-quantity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CalculateQuantity([FromBody] AddDispensingCalculationDto[] calculations)
        {
            try
            {
                var result = await dispensaryRepository.SetQuantities(calculations, HttpContext.RequestAborted);
                if (result < 1)
                    return BadRequest(new { message = "No quantities were calculated. Please check the input and try again." });
                return Ok(new { message = "Quantities calculated successfully.", recordsAffected = result });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error calculating quantities");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error calculating quantities");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during quantity calculation." });
            }
        }

        [HttpPut("update-calculation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCalculation([FromBody] UpdateDispensingCalculationDto[] calculations)
        {
            try
            {
                var result = await dispensaryRepository.EditCalculation(calculations, HttpContext.RequestAborted);
                if (result < 1)
                    return BadRequest(new { message = "No quantities were updated. Please verify and try again." });
                return Ok(new { message = "Quantities updated successfully.", recordsAffected = result });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error updating quantities");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating quantities");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during quantity update." });
            }
        }

        #endregion

        #region payments

        /// <summary>
        /// Phase 3: Get payment information for a dispensing calculation
        /// </summary>
        [HttpGet("payment/{drugsRequestId:guid}")]
        [ProducesResponseType(typeof(DrugPaymentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DrugPaymentDTO>> GetPayment(Guid drugsRequestId)
        {
            try
            {
                // TODO: Implement get payment from repository
                return StatusCode(StatusCodes.Status501NotImplemented, new { message = "Get payment endpoint not yet implemented." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error retrieving payment {DrugsRequestId}", drugsRequestId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving payment {DrugsRequestId}", drugsRequestId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the payment." });
            }
        }

        /// <summary>
        /// Phase 3: Accounts receive payments for the prescription
        /// </summary>
        [HttpPost("payment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddDrugPayment([FromBody] AddDrugPaymentDto request)
        {
            (Guid id, string user) info = (Guid.CreateVersion7(), User.Identity?.Name ?? "UnknownUser");

            try
            {
                // TODO: Implement drug payment in the repository
                return StatusCode(StatusCodes.Status501NotImplemented, new { message = "Drug payment endpoint not yet implemented." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error recording payment");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error recording payment");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during payment recording." });
            }
        }

        [HttpPut("payment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateDrugPayment([FromBody] UpdateDrugPaymentDto request)
        {
            try
            {
                // TODO: Implement drug payment update in the repository
                return StatusCode(StatusCodes.Status501NotImplemented, new { message = "Drug payment update endpoint not yet implemented." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error updating payment");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating payment");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during payment update." });
            }
        }

        #endregion

        #region dispensing

        /// <summary>
        /// Phase 4: Get dispensing information for a drug request
        /// </summary>
        [HttpGet("dispense/{drugsRequestId:guid}")]
        [ProducesResponseType(typeof(DispensingDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DispensingDetailDto>> GetDispensingDetails(Guid drugsRequestId)
        {
            try
            {
                // TODO: Implement get dispensing details from repository
                return StatusCode(StatusCodes.Status501NotImplemented, new { message = "Get dispensing details endpoint not yet implemented." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error retrieving dispensing {DrugsRequestId}", drugsRequestId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving dispensing {DrugsRequestId}", drugsRequestId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving dispensing information." });
            }
        }

        /// <summary>
        /// Phase 4: Pharmacy issues/dispenses the drug if quantity to dispense is greater than 0
        /// </summary>
        [HttpPost("dispense")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DispenserDrug([FromBody] AddDispensingDto request)
        {
            (Guid id, string user) info = (Guid.CreateVersion7(), User.Identity?.Name ?? "UnknownUser");

            try
            {
                // TODO: Implement dispensing in the repository
                return StatusCode(StatusCodes.Status501NotImplemented, new { message = "Drug dispensing endpoint not yet implemented." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error dispensing drug");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error dispensing drug");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during drug dispensing." });
            }
        }

        [HttpPut("dispense")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateDispenserDrug([FromBody] UpdateDispensingDto request)
        {
            try
            {
                // TODO: Implement dispensing update in the repository
                return StatusCode(StatusCodes.Status501NotImplemented, new { message = "Drug dispensing update endpoint not yet implemented." });
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error updating dispensing");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a database level error." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating dispensing");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during dispensing update." });
            }
        }

        #endregion
    }
}
