using Dapper;
using Npgsql;
using ShimsServer.Models.Schemes;

namespace ShimsServer.Data.Repositories
{
    /// <summary>
    /// Repository abstraction for scheme drugs database operations
    /// Allows testing without direct NpgsqlDataSource dependency
    /// Performance cost: negligible (one method call vs direct instantiation)
    /// </summary>
    public interface ISchemeDrugsRepository
    {
        /// <summary>
        /// Opens a new database connection asynchronously
        /// </summary>
        Task<NpgsqlConnection> GetConnectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all active drugs for a specific scheme
        /// </summary>
        Task<IEnumerable<SchemeDrugDTO>> GetDrugsBySchemeAsync(Guid schemeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new scheme drug pricing within a transaction
        /// Automatically deactivates previous pricing for the same scheme/drug combination
        /// </summary>
        Task<Guid> AddSchemeDrugAsync(
            Guid schemeId,
            Guid drugId,
            decimal price,
            string userName,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Default implementation wrapping NpgsqlDataSource
    /// </summary>
    public class SchemeDrugsRepository(NpgsqlDataSource dataSource) : ISchemeDrugsRepository
    {
        private readonly NpgsqlDataSource _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

        public async Task<NpgsqlConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            return await _dataSource.OpenConnectionAsync(cancellationToken);
        }

        public async Task<IEnumerable<SchemeDrugDTO>> GetDrugsBySchemeAsync(Guid schemeId, CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT sd.schemedrugsid, d.drug, sd.price, sd.drugcode, d.tags, d.description
                FROM schemedrugs sd
                INNER JOIN schemes s ON sd.schemesid = s.schemesid
                INNER JOIN drugs d ON sd.drugsid = d.drugsid
                WHERE sd.schemesid = @schemeId AND sd.isactive
                """;
            
            await using var connection = await GetConnectionAsync(cancellationToken);
            return await connection.QueryAsync<SchemeDrugDTO>(sql, new { schemeId });
        }

        public async Task<Guid> AddSchemeDrugAsync(
            Guid schemeId,
            Guid drugId,
            decimal price,
            string userName,
            CancellationToken cancellationToken = default)
        {
            var schemeDrugId = Guid.CreateVersion7();

            await using var connection = await GetConnectionAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            try
            {
                // Deactivate previous pricing
                using (var cmd = connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = """
                        UPDATE schemedrugs
                        SET isactive = false
                        WHERE schemesid = @schemeId AND drugsid = @drugId AND isactive;
                        """;
                    cmd.Parameters.Add(new NpgsqlParameter("@schemeId", schemeId));
                    cmd.Parameters.Add(new NpgsqlParameter("@drugId", drugId));
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }

                // Insert new pricing record
                using (var cmd = connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = """
                        INSERT INTO schemedrugs (schemedrugsid, schemesid, drugsid, price, dateset, isactive, username)
                        VALUES (@schemeDrugId, @schemeId, @drugId, @price, now(), true, @userName);
                        """;
                    cmd.Parameters.Add(new NpgsqlParameter("@schemeDrugId", schemeDrugId));
                    cmd.Parameters.Add(new NpgsqlParameter("@schemeId", schemeId));
                    cmd.Parameters.Add(new NpgsqlParameter("@drugId", drugId));
                    cmd.Parameters.Add(new NpgsqlParameter("@price", price));
                    cmd.Parameters.Add(new NpgsqlParameter("@userName", userName));
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return schemeDrugId;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
