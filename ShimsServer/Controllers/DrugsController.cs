using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ShimsServer.Models.Drugs;
using ShimsServer.Repositories;

namespace ShimsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DrugsController(IDrugsRepository repository, ILogger<DrugsController> logger) : ControllerBase
    {
        /// <summary>
        /// Get all drugs
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DrugsDTO>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<DrugsDTO>> GetDrugs() => await repository.Drugs(HttpContext.RequestAborted);

        /// <summary>
        /// Check if a drug exists by name
        /// </summary>
        [HttpGet("check/{drug:required:alpha}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> DrugExists(string drug)
        {
            return await repository.DrugExists(drug, HttpContext.RequestAborted) ? Ok() : NotFound(new { Message = $"{drug} not found" });
        }

        /// <summary>
        /// Add a new drug
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddDrug([FromBody] AddDrugDto drugDto)
        {
            try
            {
                var id = Guid.CreateVersion7();
                var res = await repository.AddDrug(id, drugDto, HttpContext.RequestAborted);

                return res == 1 ? Ok(id) : BadRequest(new { message = "Failed to add drug." });
            }
            catch (PostgresException pex)
            {
                logger.LogError(pex.Message, "Error adding drug");
                return BadRequest(new { Message = "An error occurred while adding the drug." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding drug");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while adding the drug." });
            }
        }

        /// <summary>
        /// Update a drug
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDrug([FromBody] UpdateDrugDto drug)
        {
            try
            {
                if (!await repository.DrugExists(drug.DrugsID, HttpContext.RequestAborted))
                    return BadRequest(new { Message = "Drug not found" });
                var result = await repository.EditDrug(drug, HttpContext.RequestAborted);

                return result == 1 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError, new { message = "Drug not found." });
            }
            catch (PostgresException pex)
            {
                logger.LogError(pex, "Error updating drug");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the drug." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating drug");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the drug." });
            }
        }

        /// <summary>
        /// Delete a drug
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDrug(Guid id)
        {
            try
            {
                if (!await repository.DrugExists(id, HttpContext.RequestAborted))
                    return BadRequest(new { Message = "Drug not found" });
                var n = await repository.DeleteDrug(id, HttpContext.RequestAborted);
                return n == 1 ?
                    Ok():
                BadRequest(new { message = "Drug not found." });
            }
            catch (PostgresException pex)
            {
                logger.LogError(pex, "Error deleting drug");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the drug." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting drug");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the drug." });
            }
        }
    }
}
