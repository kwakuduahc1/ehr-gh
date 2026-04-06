using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShimsServer.Context;
using ShimsServer.Models.Schemes;
using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SchemesController(DbContextOptions<ApplicationDbContext> opitons, CancellationToken token) : ControllerBase
    {
        private readonly ApplicationDbContext db = new(opitons);

        /// <summary>
        /// Get all schemes
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SchemesDTO>>> GetSchemes()
        {
            var schemes = await db.Schemes
                .Select(x => new SchemesDTO(
                    x.SchemesID,
                    x.SchemeName,
                    x.Coverage,
                    x.MaxPayable,
                    x.Recovery
                ))
                .ToListAsync(token);

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
            var scheme = await db.Schemes
                .Where(p => p.SchemesID == id)
                .Select(p => new SchemesDTO(
                    p.SchemesID,
                    p.SchemeName,
                    p.Coverage,
                    p.MaxPayable,
                    p.Recovery
                ))
                .FirstOrDefaultAsync(token);

            return scheme == null ? NotFound() : Ok(scheme);
        }

        /// <summary>
        /// Check if a scheme exists by name
        /// </summary>
        private async Task<bool> SchemeExists(string name)
        {
            var exists = await db.Schemes.AnyAsync(x => x.SchemeName == name, token);
            return exists;
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
           if(await this.SchemeExists(schemeDto.SchemeName))
                return Conflict(new { message = $"Scheme with name {schemeDto.SchemeName} already exists." });

            var scheme = new Schemes
            {
                SchemesID = Guid.CreateVersion7(),
                SchemeName = schemeDto.SchemeName,
                Coverage = schemeDto.Coverage,
                MaxPayable = schemeDto.MaxPayable,
                Recovery = schemeDto.Recovery
            };

            db.Schemes.Add(scheme);
            await db.SaveChangesAsync(token);
            return Ok(scheme.SchemesID);
        }

        /// <summary>
        /// Update an existing scheme
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateScheme(UpdateSchemeDto scheme)
        {
            var _scheme = await db.Schemes.FindAsync(scheme.SchemesID);
            if (_scheme == null)
                return BadRequest(new {Message =  $"Scheme {scheme.SchemeName} does not exists"});
            _scheme.SchemeName = scheme.SchemeName;
            _scheme.Coverage = scheme.Coverage;
            _scheme.MaxPayable = scheme.MaxPayable;
            _scheme.Recovery = scheme.Recovery;
            db.Entry(_scheme).State = EntityState.Modified;
            await db.SaveChangesAsync(token);
            return Accepted();
        }

        /// <summary>
        /// Delete a scheme by ID
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteScheme(Guid id)
        {
            var scheme = await db.Schemes.FindAsync(id, token);
            if (scheme == null)
                return NotFound();
            db.Schemes.Remove(scheme);
            await db.SaveChangesAsync(token);
            return Ok();
        }
    }

    public record SchemesDTO(Guid SchemesID, string SchemeName, string Coverage, decimal MaxPayable, decimal Recovery);

    public record AddSchemeDto(
        string SchemeName,
        [StringLength(30, MinimumLength = 3), AllowedValues(["Full", "Relative", "Fixed"])] string Coverage,
        [Range(0.01, double.MaxValue, ErrorMessage = "MaxPayable must be greater than 0")] decimal MaxPayable,
        [Range(0, double.MaxValue)] decimal Recovery);

    public record UpdateSchemeDto(
        Guid SchemesID,
        string SchemeName,
        [StringLength(30, MinimumLength = 3), AllowedValues(["Full", "Relative", "Fixed"])] string Coverage,
        [Range(0.01, double.MaxValue, ErrorMessage = "MaxPayable must be greater than 0")] decimal MaxPayable,
        [Range(0, double.MaxValue)] decimal Recovery);
}
