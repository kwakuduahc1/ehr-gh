using Npgsql;

namespace ShimsServer.Data.Repositories
{
    /// <summary>
    /// Repository abstraction for database operations
    /// Allows testing without direct NpgsqlDataSource dependency
    /// Performance cost: negligible (one method call vs direct instantiation)
    /// </summary>
    public interface ISchemeServiceRepository
    {
        /// <summary>
        /// Opens a new database connection asynchronously
        /// </summary>
        Task<NpgsqlConnection> GetConnectionAsync(CancellationToken cancellationToken = default);

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
    /// Default implementation wrapping NpgsqlDataSource
    /// </summary>
    public class SchemeServiceRepository(NpgsqlDataSource dataSource) : ISchemeServiceRepository
    {
        private readonly NpgsqlDataSource _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

        public async Task<NpgsqlConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            return await _dataSource.OpenConnectionAsync(cancellationToken);
        }

        public async Task<int> ExecuteCommandAsync(
            string commandText,
            Dictionary<string, object?>? parameters = null,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await GetConnectionAsync(cancellationToken);
            using var command = connection.CreateCommand();

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
            await using var connection = await GetConnectionAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await operation(connection, transaction, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}

