namespace DataSharedLib.Connection;

using Microsoft.Data.SqlClient;

public interface IDatabaseConnectionFactory
{
    SqlConnection CreateConnection();
    Task<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);
}
