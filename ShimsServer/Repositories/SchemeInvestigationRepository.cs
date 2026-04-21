using Dapper;
using Npgsql;
using ShimsServer.Models.Schemes;

namespace ShimsServer.Repositories
{
    /// <summary>
    /// Repository abstraction for scheme investigation database operations
    /// Provides explicit intent for investigation pricing management
    /// </summary>
    public interface ISchemeInvestigationRepository
    {
        /// <summary>
        /// Gets all active investigations for a specific scheme
        /// </summary>
        Task<IEnumerable<SchemeInvestigationDTO>> GetInvestigationsBySchemeAsync(
            Guid schemeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new scheme investigation pricing within a transaction
        /// Automatically deactivates previous pricing for the same scheme/investigation combination
        /// </summary>
        Task<Guid> AddSchemeInvestigationAsync(
            AddSchemeInvestigationDto dto,
            string userName,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Default implementation wrapping IConnection
    /// </summary>
    public class SchemeInvestigationRepository(IConnection connection) : ISchemeInvestigationRepository
    {
        public async Task<IEnumerable<SchemeInvestigationDTO>> GetInvestigationsBySchemeAsync(
            Guid schemeId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT schemeinvestigationsid, investigationsid, schemesid, gdrg, price, isactive, investigation, narration
                FROM public.vwm_investigations
                WHERE schemesid = @id;
                """;
            await using var conn = await connection.ConnectionAsync(cancellationToken);
            return await conn.QueryAsync<SchemeInvestigationDTO>(sql, new { id = schemeId });
        }

        public async Task<Guid> AddSchemeInvestigationAsync(
            AddSchemeInvestigationDto dto,
            string userName,
            CancellationToken cancellationToken = default)
        {
            var siID = Guid.CreateVersion7();

            await using var conn = await connection.ConnectionAsync(cancellationToken);
            await using var tran = await conn.BeginTransactionAsync(cancellationToken);

            // Deactivate previous pricing
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = """
                    UPDATE schemeinvestigations
                    SET isactive = false
                    WHERE schemesid = @id AND investigationsid = @iid AND isactive;
                    """;
                cmd.Parameters.Add(new NpgsqlParameter("@id", dto.SchemesID));
                cmd.Parameters.Add(new NpgsqlParameter("@iid", dto.InvestigationsID));
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }

            // Insert new pricing record
            using var cmd2 = conn.CreateCommand();
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
            cmd2.Parameters.Add(new NpgsqlParameter("@user", userName));
            cmd2.Parameters.Add(new NpgsqlParameter("@gdrg", dto.GDRG));
            cmd2.Parameters.Add(new NpgsqlParameter("@narration", dto.Narration));
            await cmd2.ExecuteNonQueryAsync(cancellationToken);
            await tran.CommitAsync(cancellationToken);

            return siID;
        }
    }
}
