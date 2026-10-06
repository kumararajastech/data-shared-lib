namespace DataSharedLib.Connection;

using Microsoft.Data.SqlClient;

public interface IDatabaseConnectionFactory
{
    Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
    SqlConnection CreateConnection();
}
