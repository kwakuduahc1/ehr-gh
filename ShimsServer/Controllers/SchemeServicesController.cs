using Microsoft.AspNetCore.Mvc;
using ShimsServer.Models.Schemes;
using ShimsServer.Data.Repositories;
using Dapper;
using Npgsql;
using Microsoft.AspNetCore.Authorization;

namespace ShimsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    //[Authorize(Policy = "SysAdmin")]
    public class SchemeServicesController(ISchemeServiceRepository repository, ILogger<SchemeServicesController> logger, CancellationToken token) : ControllerBase
    {

        /// <summary>
        /// Get services for a specific scheme
        /// </summary>
        [HttpGet("scheme/{id:guid}")]
        [ProducesResponseType(typeof(IEnumerable<SchemeServiceDTO>), StatusCodes.Status200OK)]

        [ResponseCache(Duration = 8640*20, Location = ResponseCacheLocation.Client, VaryByQueryKeys = ["id"] )]
        public async Task<IEnumerable<SchemeServiceDTO>> GetServicesByScheme(Guid id)
        {
            const string sql = """
                SELECT schemeservicesid, servicesid, price, tiers, gdrg, narration, service, servicegroup
                FROM public.vwm_services;
                """;
            await using var connection = await repository.GetConnectionAsync(token);
            return await connection.QueryAsync<SchemeServiceDTO>(sql, new { id });
        }

        /// <summary>
        /// Create a new scheme service pricing
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> AddSchemeService([FromBody] AddSchemeServiceDto dto)
        {
            var scID = Guid.CreateVersion7();

            try
            {
                await using var con = await repository.GetConnectionAsync(token);
                await using var tran = await con.BeginTransactionAsync(token);

                // Deactivate previous pricing
                using (var cmd = con.CreateCommand())
                {
                    cmd.Transaction = tran;
                    cmd.CommandText = """
                        UPDATE schemeservices
                        SET isactive = false
                        WHERE schemesid = @id AND servicesid = @did AND isactive;
                        """;
                    cmd.Parameters.Add(new NpgsqlParameter("@id", dto.SchemesID));
                    cmd.Parameters.Add(new NpgsqlParameter("@did", dto.ServicesID));
                    await cmd.ExecuteNonQueryAsync(token);
                }

                // Insert new pricing record
                using var cmd2 = con.CreateCommand();
                cmd2.Transaction = tran;
                cmd2.CommandText = """
                        INSERT INTO public.schemeservices(
                        schemeservicesid, schemesid, servicesid, price, dateset, username, allowedtiers, isactive, gdrg, narration)
                        VALUES (@sdid, @sid, @did, @price, now(), @user, @tiers, true, @gdrg, @narration);
                        """;
                cmd2.Parameters.Add(new NpgsqlParameter("@sdid", scID));
                cmd2.Parameters.Add(new NpgsqlParameter("@sid", dto.SchemesID));
                cmd2.Parameters.Add(new NpgsqlParameter("@did", dto.ServicesID));
                cmd2.Parameters.Add(new NpgsqlParameter("@price", dto.Price));
                cmd2.Parameters.Add(new NpgsqlParameter("@user", User.Identity!.Name));
                cmd2.Parameters.Add(new NpgsqlParameter("@tiers", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Varchar) { Value = dto.AllowedTiers });
                cmd2.Parameters.Add(new NpgsqlParameter("@gdrg", dto.GDRG));
                cmd2.Parameters.Add(new NpgsqlParameter("@narration", dto.Narration));  
                await cmd2.ExecuteNonQueryAsync(token);
                await tran.CommitAsync(token);
                return Ok(scID);
            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "Database error occurred while adding scheme service pricing");
                return BadRequest(new { message = "A database error occurred." });
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex, "Operation cancelled while adding scheme service pricing");
                return BadRequest(new { message = "The operation was cancelled." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while adding scheme service pricing");
                return BadRequest(new { message = "An unexpected error occurred." });
            }
        }
    }
}
