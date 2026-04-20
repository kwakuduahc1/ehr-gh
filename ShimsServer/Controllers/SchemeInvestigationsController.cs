using Microsoft.AspNetCore.Mvc;
using ShimsServer.Models.Schemes;
using Dapper;
using Npgsql;
using Microsoft.AspNetCore.Authorization;
using ShimsServer.Repositories;

namespace ShimsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    //[Authorize(Policy = "SysAdmin")]
    public class SchemeInvestigationsController(ISchemeServiceRepository repository, ILogger<SchemeInvestigationsController> logger, CancellationToken token) : ControllerBase
    {

        /// <summary>
        /// Get investigations for a specific scheme
        /// </summary>
        [HttpGet("scheme/{id:guid}")]
        [ProducesResponseType(typeof(IEnumerable<SchemeInvestigationDTO>), StatusCodes.Status200OK)]
        [ResponseCache(Duration = 8640 * 20, Location = ResponseCacheLocation.Client, VaryByQueryKeys = ["id"])]
        public async Task<IEnumerable<SchemeInvestigationDTO>> GetInvestigationsByScheme(Guid id)
        {
            const string sql = """
                SELECT schemeinvestigationsid, investigationsid, schemesid, gdrg, price, isactive, investigation, narration
                FROM public.vwm_investigations
                WHERE schemesid = @id;
                """;
            await using var connection = await repository.GetConnectionAsync(token);
            return await connection.QueryAsync<SchemeInvestigationDTO>(sql, new { id });
        }

        /// <summary>
        /// Create a new scheme investigation pricing
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> AddSchemeInvestigation([FromBody] AddSchemeInvestigationDto dto)
        {
            var siID = Guid.CreateVersion7();

            try
            {
                await using var con = await repository.GetConnectionAsync(token);
                await using var tran = await con.BeginTransactionAsync(token);

                // Deactivate previous pricing
                using (var cmd = con.CreateCommand())
                {
                    cmd.Transaction = tran;
                    cmd.CommandText = """
                        UPDATE schemeinvestigations
                        SET isactive = false
                        WHERE schemesid = @id AND investigationsid = @iid AND isactive;
                        """;
                    cmd.Parameters.Add(new NpgsqlParameter("@id", dto.SchemesID));
                    cmd.Parameters.Add(new NpgsqlParameter("@iid", dto.InvestigationsID));
                    await cmd.ExecuteNonQueryAsync(token);
                }

                // Insert new pricing record
                using var cmd2 = con.CreateCommand();
                cmd2.Transaction = tran;
                cmd2.CommandText = """
                        INSERT INTO public.schemeinvestigations(
                        schemeinvestigationsid, schemesid, investigationsid, price, dateset, username, gdrg, isactive, narration)
                        VALUES (@siid, @sid, @iid, @price, now(), @user, @gdrg, true, @narration);
                        """;
                cmd2.Parameters.Add(new NpgsqlParameter("@siid", siID));
                cmd2.Parameters.Add(new NpgsqlParameter("@sid", dto.SchemesID));
                cmd2.Parameters.Add(new NpgsqlParameter("@iid", dto.InvestigationsID));
                cmd2.Parameters.Add(new NpgsqlParameter("@price", dto.Price));
                cmd2.Parameters.Add(new NpgsqlParameter("@user", User.Identity!.Name));
                cmd2.Parameters.Add(new NpgsqlParameter("@gdrg", dto.GDRG));
                cmd2.Parameters.Add(new NpgsqlParameter("@narration", dto.Narration ?? (object)DBNull.Value));
                await cmd2.ExecuteNonQueryAsync(token);
                await tran.CommitAsync(token);
                return CreatedAtAction(nameof(GetInvestigationsByScheme), new { id = dto.SchemesID }, siID);
            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "Database error occurred while adding scheme investigation pricing");
                return BadRequest(new { message = "A database error occurred." });
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex, "Operation cancelled while adding scheme investigation pricing");
                return BadRequest(new { message = "The operation was cancelled." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while adding scheme investigation pricing");
                return BadRequest(new { message = "An unexpected error occurred." });
            }
        }
    }
}
