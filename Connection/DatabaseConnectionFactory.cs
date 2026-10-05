using System.Data;
using DataSharedLib.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace DataSharedLib.Connection;

public class DatabaseConnectionFactory : IDatabaseConnectionFactory
{
    private readonly DatabaseConnectionOptions _options;

    public DatabaseConnectionFactory(IOptions<DatabaseConnectionOptions> options)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new ArgumentException("ConnectionString must be provided in DatabaseConnectionOptions.", nameof(options));
        }
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_options.ConnectionString);
    }

    public async Task<SqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = CreateConnection();
        try
        {
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
        catch (SqlException ex)
        {
            await connection.DisposeAsync();
            throw new ConnectionException($"Failed to open connection to SQL Server: {ex.Message}", ex, ex.Number);
        }
    }
}
