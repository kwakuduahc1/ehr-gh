using Dapper;
using Npgsql;
using ShimsServer.Models.DTOs;
using ShimsServer.Models.Schemes;

namespace ShimsServer.Repositories
{
    /// <summary>
    /// Repository abstraction for scheme database operations
    /// Allows testing without direct NpgsqlDataSource dependency
    /// Performance cost: negligible (one method call vs direct instantiation)
    /// </summary>
    public interface ISchemesRepository
    {
        /// <summary>
        /// Gets all active schemes
        /// </summary>
        Task<IEnumerable<SchemesDTO>> GetAllSchemesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a specific scheme by ID if it's active
        /// </summary>
        Task<SchemesDTO?> GetSchemeByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a scheme exists by name
        /// </summary>
        Task<bool> SchemeExistsByNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new scheme within a transaction
        /// </summary>
        Task<Guid> AddSchemeAsync(Schemes scheme, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing scheme within a transaction
        /// </summary>
        Task<bool> UpdateSchemeAsync(Schemes scheme, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a scheme by ID within a transaction
        /// </summary>
        Task<bool> DeleteSchemeAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a scheme exists by ID
        /// </summary>
        Task<bool> SchemeExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Default implementation wrapping IConnection
    /// </summary>
    public class SchemesRepository(IConnection connection) : ISchemesRepository
    {
        public async Task<IEnumerable<SchemesDTO>> GetAllSchemesAsync(CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT schemesid, schemename, coverage, maxpayable, recovery
                FROM vwm_schemes
                """;
            await using var conn = await connection.ConnectionAsync(cancellationToken);
            return await conn.QueryAsync<SchemesDTO>(sql);
        }

        public async Task<SchemesDTO?> GetSchemeByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT schemesid, schemename, coverage, maxpayable, recovery
                FROM schemes
                WHERE schemesid = @id AND isactive = true
                """;
            await using var conn = await connection.ConnectionAsync(cancellationToken);
            return await conn.QueryFirstOrDefaultAsync<SchemesDTO?>(sql, new { id });
        }

        public async Task<bool> SchemeExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT EXISTS (
                    SELECT 1
                    FROM schemes
                    WHERE SchemeName = @name
                )
                """;
            await using var conn = await connection.ConnectionAsync(cancellationToken);
            return await conn.ExecuteScalarAsync<bool>(sql, new { name });
        }

        public async Task<Guid> AddSchemeAsync(Schemes scheme, CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO schemes (schemesid, schemename, coverage, maxpayable, recovery, isactive)
                VALUES (@SchemesID, @SchemeName, @Coverage, @MaxPayable, @Recovery, true)
                """;

            await using var conn = await connection.ConnectionAsync(cancellationToken);
            await using var transaction = await conn.BeginTransactionAsync(cancellationToken);

            await conn.ExecuteAsync(sql, scheme, transaction: transaction);
            await transaction.CommitAsync(cancellationToken);
            return scheme.SchemesID;
        }

        public async Task<bool> UpdateSchemeAsync(Schemes scheme, CancellationToken cancellationToken = default)
        {
            const string checkSql = """
                SELECT SchemesID, SchemeName, Coverage, MaxPayable, Recovery
                FROM schemes
                WHERE schemesid = @SchemesID
                """;

            const string updateSql = """
                UPDATE schemes
                SET schemename = @SchemeName, 
                    coverage = @Coverage, 
                    maxpayable = @MaxPayable, 
                    recovery = @Recovery,
                    isactive = true
                WHERE schemesid = @SchemesID
                """;

            await using var conn = await connection.ConnectionAsync(cancellationToken);
            await using var transaction = await conn.BeginTransactionAsync(cancellationToken);

            var existingScheme = await conn.QueryFirstOrDefaultAsync<Schemes>(
                checkSql, 
                new { scheme.SchemesID }, 
                transaction: transaction);

            if (existingScheme == null)
                return false;

            await conn.ExecuteAsync(updateSql, scheme, transaction: transaction);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteSchemeAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string checkSql = "SELECT EXISTS (SELECT 1 FROM schemes WHERE schemesid = @id)";
            const string deleteSql = """
                    UPDATE schemes 
                    SET isactive = false
                    WHERE schemesid = @id
                """;

            await using var conn = await connection.ConnectionAsync(cancellationToken);
            await using var transaction = await conn.BeginTransactionAsync(cancellationToken);

            var exists = await conn.ExecuteScalarAsync<bool>(checkSql, new { id }, transaction: transaction);
            if (!exists)
                return false;

            await conn.ExecuteAsync(deleteSql, new { id }, transaction: transaction);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }

        public async Task<bool> SchemeExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "SELECT EXISTS (SELECT 1 FROM schemes WHERE schemesid = @id)";
            await using var conn = await connection.ConnectionAsync(cancellationToken);
            return await conn.ExecuteScalarAsync<bool>(sql, new { id });
        }
    }
}
