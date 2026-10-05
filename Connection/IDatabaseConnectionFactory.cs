using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace DataSharedLib.Connection;

public interface IDatabaseConnectionFactory
{
    SqlConnection CreateConnection();
    Task<SqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
