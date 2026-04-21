using Npgsql;

namespace ShimsServer.Repositories
{
    /// <summary>
    /// Repository abstraction for database operations
    /// Allows testing without direct NpgsqlDataSource dependency
    /// Performance cost: negligible (one method call vs direct instantiation)
    /// </summary>
    public interface ISchemeServiceRepository
    {
        /// <summary>
        /// Executes a query that returns no results (INSERT, UPDATE, DELETE)
        /// </summary>
        Task<int> ExecuteCommandAsync(
            string commandText,
            Dictionary<string, object?>? parameters = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a query within a transaction
        /// </summary>
        Task<T> ExecuteInTransactionAsync<T>(
            Func<NpgsqlConnection, NpgsqlTransaction, CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Default implementation wrapping IConnection
    /// </summary>
    public class SchemeServiceRepository(IConnection connection) : ISchemeServiceRepository
    {
        public async Task<int> ExecuteCommandAsync(
            string commandText,
            Dictionary<string, object?>? parameters = null,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await connection.ConnectionAsync(cancellationToken);
            using var command = conn.CreateCommand();

            command.CommandText = commandText;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task<T> ExecuteInTransactionAsync<T>(
            Func<NpgsqlConnection, NpgsqlTransaction, CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await connection.ConnectionAsync(cancellationToken);
            await using var transaction = await conn.BeginTransactionAsync(cancellationToken);

            var result = await operation(conn, transaction, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
    }
}

