using Microsoft.AspNetCore.Mvc;
using ShimsServer.Models.Schemes;
using Dapper;
using Npgsql;
using Microsoft.AspNetCore.Authorization;

namespace ShimsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    //[Authorize(Policy = "SysAdmin")]
    public class SchemeDrugsController(NpgsqlDataSource ds, ILogger<SchemeDrugsController> logger, CancellationToken token) : ControllerBase
    {

        /// <summary>
        /// Get drugs for a specific scheme
        /// </summary>
        [HttpGet("scheme/{id:guid}")]
        [ProducesResponseType(typeof(IEnumerable<SchemeDrugDTO>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<SchemeDrugDTO>> GetDrugsByScheme(Guid id)
        {
            const string sql = """
                SELECT sd.schemedrugsid, d.drug, sd.price, sd.drugcode, d.tags, d.description
                FROM schemedrugs sd
                INNER JOIN schemes s ON sd.schemesid = s.schemesid
                INNER JOIN drugs d ON sd.drugsid = d.drugsid
                WHERE sd.schemesid = @id AND sd.isactive
                """;
            await using var connection = await ds.OpenConnectionAsync(token);
            return await connection.QueryAsync<SchemeDrugDTO>(sql, new { id });
        }

        /// <summary>
        /// Create a new scheme drug pricing
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> AddSchemeDrug([FromBody] AddSchemeDrugDto schemeDrugDto)
        {
            var scID = Guid.CreateVersion7();
            var userName = User.Identity!.Name;

            await using var con = await ds.OpenConnectionAsync(token);
            await using var tran = await con.BeginTransactionAsync(token);
            try
            {
                // Deactivate previous pricing
                using (var cmd = con.CreateCommand())
                {
                    cmd.Transaction = tran;
                    cmd.CommandText = """
                        UPDATE schemedrugs
                        SET isactive = false
                        WHERE schemesid = @id AND drugsid = @did AND isactive;
                        """;
                    cmd.Parameters.Add(new NpgsqlParameter("@id", schemeDrugDto.SchemesID));
                    cmd.Parameters.Add(new NpgsqlParameter("@did", schemeDrugDto.DrugsID));
                    await cmd.ExecuteNonQueryAsync(token);
                }

                // Insert new pricing record
                using var cmd2 = con.CreateCommand();
                    cmd2.Transaction = tran;
                cmd2.CommandText = """
                        INSERT INTO schemedrugs (schemedrugsid, schemesid, drugsid, price, dateset, isactive, username)
                        VALUES (@sdid, @sid, @did, @price, now(), true, @user);
                        """;
                cmd2.Parameters.Add(new NpgsqlParameter("@sdid", scID));
                cmd2.Parameters.Add(new NpgsqlParameter("@sid", schemeDrugDto.SchemesID));
                cmd2.Parameters.Add(new NpgsqlParameter("@did", schemeDrugDto.DrugsID));
                cmd2.Parameters.Add(new NpgsqlParameter("@price", schemeDrugDto.Price));
                cmd2.Parameters.Add(new NpgsqlParameter("@user", userName));
                await cmd2.ExecuteNonQueryAsync(token);
                await tran.CommitAsync(token);
                return Ok(scID);
            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "Database error occurred while adding scheme drug pricing");
                await tran.RollbackAsync(token);
                return BadRequest(new { message = "A database error occurred." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while adding scheme drug pricing");
                await tran.RollbackAsync(token);
                return BadRequest(new { message = "An unexpected error occurred." });
            }
        }
    }
}
