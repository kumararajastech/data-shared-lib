using System.Data;
using System.Diagnostics;
using DataSharedLib.Connection;
using DataSharedLib.Exceptions;
using DataSharedLib.Helpers;
using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;
using DataSharedLib.Validation;
using Microsoft.Data.SqlClient;

namespace DataSharedLib.Services;

public class DatabaseQueryService : IDatabaseQueryService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly IRequestValidator _validator;

    public DatabaseQueryService(IDatabaseConnectionFactory connectionFactory, IRequestValidator validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<QueryResponse<IDictionary<string, object?>>> ExecuteQueryAsync(ExecuteQueryRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateExecuteQuery(request);
        var sw = Stopwatch.StartNew();

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = request.CommandText;
        if (request.TimeoutSeconds.HasValue) command.CommandTimeout = request.TimeoutSeconds.Value;
        command.Parameters.AddRange(SqlParameterHelper.CreateParameters(request.Parameters));

        var results = new List<IDictionary<string, object?>>();
        try
        {
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var row = new Dictionary<string, object?>(reader.FieldCount);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                results.Add(row);
            }
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"Query execution failed: {ex.Message}", request.CommandText, ex, ex.Number);
        }

        sw.Stop();
        return new QueryResponse<IDictionary<string, object?>>
        {
            Records = results,
            TotalCount = results.Count,
            ElapsedTime = sw.Elapsed
        };
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, IDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(SqlParameterHelper.CreateParameters(parameters));

        try
        {
            var result = await command.ExecuteScalarAsync(cancellationToken);
            if (result == null || result == DBNull.Value) return default;
            return (T)Convert.ChangeType(result, typeof(T));
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"ExecuteScalar failed: {ex.Message}", sql, ex, ex.Number);
        }
    }
}
