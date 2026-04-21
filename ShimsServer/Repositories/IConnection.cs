using Elfie.Serialization;
using Npgsql;

namespace ShimsServer.Repositories
{
    public interface IConnection
    {
        Task<NpgsqlConnection> ConnectionAsync(CancellationToken cancellationToken = default);
    }

    public class Connection(NpgsqlDataSource dsource) : IConnection
    {
        public async Task<NpgsqlConnection> ConnectionAsync(CancellationToken cancellationToken = default)
        {
            var connection = dsource.CreateConnection();
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
    }
}
