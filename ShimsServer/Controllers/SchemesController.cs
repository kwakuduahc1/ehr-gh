using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ShimsServer.Models.Schemes;
using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SchemesController(NpgsqlDataSource dsource, ILogger<SchemesController> logger, CancellationToken token) : ControllerBase
    {

        /// <summary>
        /// Get all schemes
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]

        public async Task<ActionResult<IEnumerable<SchemesDTO>>> GetSchemes()
        {
            const string sql = """
                SELECT schemesid, schemename, coverage, maxpayable, recovery
                FROM vwm_schemes
                """;
            using var con = dsource.CreateConnection();
            var schemes = await con.QueryAsync<SchemesDTO>(sql);
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
            const string sql = """
                SELECT schemesid, schemename, coverage, maxpayable, recovery
                FROM schemes
                WHERE schemesid = @id AND isactive = true
                """;
            using var con = dsource.CreateConnection();
            var scheme = await con.QueryFirstOrDefaultAsync<SchemesDTO>(sql, new { id });
            return scheme == null ? NotFound() : Ok(scheme);
        }

        /// <summary>
        /// Check if a scheme exists by name
        /// </summary>
        private async Task<bool> SchemeExists(string name)
        {
            const string sql = """
                SELECT EXISTS (
                    SELECT 1
                    FROM schemes
                    WHERE SchemeName = @name
                )
                """;
            return await dsource.CreateConnection().ExecuteScalarAsync<bool>(sql, new { name });
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
            if (await this.SchemeExists(schemeDto.SchemeName))
                return Conflict(new { message = $"Scheme with name {schemeDto.SchemeName} already exists." });

            var scheme = new Schemes
            {
                SchemesID = Guid.CreateVersion7(),
                SchemeName = schemeDto.SchemeName,
                Coverage = schemeDto.Coverage,
                MaxPayable = schemeDto.MaxPayable,
                Recovery = schemeDto.Recovery
            };
            const string sql = """
                INSERT INTO schemes (schemesid, schemename, coverage, maxpayable, recovery, isactive)
                VALUES (@SchemesID, @SchemeName, @Coverage, @MaxPayable, @Recovery, true)
                """;
            using var con = dsource.CreateConnection();
            using var tran = await con.BeginTransactionAsync(token);
            try
            {
                var res = await con.ExecuteAsync(sql, scheme, transaction: tran);
                await tran.CommitAsync(token);
            }
            catch (PostgresException ex)
            {
                await tran.RollbackAsync(token);
                logger.LogError(ex, "Error inserting scheme {SchemeName}", schemeDto.SchemeName);
                return BadRequest(new { message = $"There was a database level error" });
            }
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
            const string sql = """
                SELECT SchemesID, SchemeName, Coverage, MaxPayable, Recovery
                FROM schemes
                WHERE schemesid = @id
                """;
            using var con = dsource.CreateConnection();
            using var tran = await con.BeginTransactionAsync(token);
            var _scheme = await con.QueryFirstOrDefaultAsync<Schemes>(sql, new { id = scheme.SchemesID }, transaction: tran);
            if (_scheme == null)
                return BadRequest(new { Message = $"Scheme {scheme.SchemeName} does not exists" });
            const string insSql = """
                UPDATE schemes
                SET schemename = @SchemeName, 
                    coverage = @Coverage, 
                    maxpayable = @MaxPayable, 
                    recovery = @Recovery,
                    isactive = true
                WHERE schemesid = @SchemesID
                """;
            try
            {
                var res = await con.ExecuteScalarAsync(insSql, scheme, transaction: tran);
                await tran.CommitAsync(token);
                return Accepted();
            }
            catch (PostgresException ex)
            {
                await tran.RollbackAsync(token);
                logger.LogError(ex, "Error updating scheme {SchemeName}", scheme.SchemeName);
                return BadRequest(new { message = $"There was a database level error" });
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
            using var con = dsource.CreateConnection(); 
            using var tran = await con.BeginTransactionAsync(token);
            try
            {
                var exists = await con.ExecuteScalarAsync<bool>("SELECT EXISTS (SELECT 1 FROM schemes WHERE schemesid = @id)", new { id }, transaction: tran);
                if (!exists)
                    return NotFound();
                const string delSql = "DELETE FROM schemes WHERE schemesid = @id";
                await con.ExecuteAsync(delSql, new { id }, transaction: tran);
                await tran.CommitAsync(token);
                return Ok();
            }
            catch (PostgresException ex)
            {
                await tran.RollbackAsync(token);
                logger.LogError(ex, "Error deleting scheme with ID {SchemeID}", id);
                return BadRequest(new { message = "There was a database level error" });
            }
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
