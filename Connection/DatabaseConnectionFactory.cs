namespace DataSharedLib.Connection;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using DataSharedLib.Exceptions;

public class DatabaseConnectionFactory : IDatabaseConnectionFactory
{
    private readonly DatabaseConnectionOptions _options;

    public DatabaseConnectionFactory(IOptions<DatabaseConnectionOptions> options)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new ConnectionException("Database connection string is not configured.");
        }
    }

    public async Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = new SqlConnection(_options.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
        catch (SqlException ex)
        {
            throw new ConnectionException($"Failed to open SQL Server connection: {ex.Message}", ex.Number, ex);
        }
    }

    public SqlConnection CreateConnection()
    {
        try
        {
            var connection = new SqlConnection(_options.ConnectionString);
            connection.Open();
            return connection;
        }
        catch (SqlException ex)
        {
            throw new ConnectionException($"Failed to open SQL Server connection: {ex.Message}", ex.Number, ex);
        }
    }
}
