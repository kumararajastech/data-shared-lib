namespace DataSharedLib.Connection;

using DataSharedLib.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

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

    public SqlConnection CreateConnection() => new(_options.ConnectionString);

    public async Task<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = CreateConnection();
        var attempts = 0;

        while (true)
        {
            try
            {
                attempts++;
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                return connection;
            }
            catch (SqlException ex) when (attempts <= _options.MaxRetryCount)
            {
                await connection.DisposeAsync().ConfigureAwait(false);
                await Task.Delay(_options.RetryIntervalMs, cancellationToken).ConfigureAwait(false);
                connection = CreateConnection();
            }
            catch (Exception ex)
            {
                await connection.DisposeAsync().ConfigureAwait(false);
                throw new ConnectionException($"Failed to open database connection after {attempts} attempt(s).", ex);
            }
        }
    }
}
