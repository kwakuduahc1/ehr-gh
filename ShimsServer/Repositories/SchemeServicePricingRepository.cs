using Dapper;
using Npgsql;
using NpgsqlTypes;
using ShimsServer.Models.Schemes;

namespace ShimsServer.Repositories
{
    /// <summary>
    /// Repository abstraction for scheme service pricing database operations
    /// Provides explicit intent for service pricing management
    /// </summary>
    public interface ISchemeServicePricingRepository
    {
        /// <summary>
        /// Gets all active services for a specific scheme
        /// </summary>
        Task<IEnumerable<SchemeServiceDTO>> GetServicesBySchemeAsync(
            Guid schemeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new scheme service pricing within a transaction
        /// Automatically deactivates previous pricing for the same scheme/service combination
        /// </summary>
        Task<Guid> AddSchemeServiceAsync(
            AddSchemeServiceDto dto,
            string userName,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Default implementation wrapping IConnection
    /// </summary>
    public class SchemeServicePricingRepository(IConnection connection) : ISchemeServicePricingRepository
    {
        public async Task<IEnumerable<SchemeServiceDTO>> GetServicesBySchemeAsync(
            Guid schemeId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT schemeservicesid, servicesid, price, tiers, gdrg, narration, service, servicegroup
                FROM public.vwm_services
                WHERE schemesid = @id;
                """;
            await using var conn = await connection.ConnectionAsync(cancellationToken);
            return await conn.QueryAsync<SchemeServiceDTO>(sql, new { id = schemeId });
        }

        public async Task<Guid> AddSchemeServiceAsync(
            AddSchemeServiceDto dto,
            string userName,
            CancellationToken cancellationToken = default)
        {
            var scID = Guid.CreateVersion7();

            await using var conn = await connection.ConnectionAsync(cancellationToken);
            await using var tran = await conn.BeginTransactionAsync(cancellationToken);

            // Deactivate previous pricing
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = """
                    UPDATE schemeservices
                    SET isactive = false
                    WHERE schemesid = @id AND servicesid = @did AND isactive;
                    """;
                cmd.Parameters.Add(new NpgsqlParameter("@id", dto.SchemesID));
                cmd.Parameters.Add(new NpgsqlParameter("@did", dto.ServicesID));
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }

            // Insert new pricing record
            using var cmd2 = conn.CreateCommand();
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
            cmd2.Parameters.Add(new NpgsqlParameter("@user", userName));
            cmd2.Parameters.Add(new NpgsqlParameter("@tiers", NpgsqlDbType.Array | NpgsqlDbType.Varchar) { Value = dto.AllowedTiers });
            cmd2.Parameters.Add(new NpgsqlParameter("@gdrg", dto.GDRG));
            cmd2.Parameters.Add(new NpgsqlParameter("@narration", dto.Narration ?? (object)DBNull.Value));
            await cmd2.ExecuteNonQueryAsync(cancellationToken);
            await tran.CommitAsync(cancellationToken);

            return scID;
        }
    }
}
