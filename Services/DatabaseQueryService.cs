namespace DataSharedLib.Services;

using System.Data;
using DataSharedLib.Connection;
using DataSharedLib.Exceptions;
using DataSharedLib.Helpers;
using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;
using DataSharedLib.Validation;
using Microsoft.Data.SqlClient;

public class DatabaseQueryService : IDatabaseQueryService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly IRequestValidator _validator;

    public DatabaseQueryService(IDatabaseConnectionFactory connectionFactory, IRequestValidator validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<QueryResponse> ExecuteQueryAsync(ExecuteQueryRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateExecuteQueryRequest(request);
        var watch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await using var connection = await _connectionFactory.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            await using var command = connection.CreateCommand();
            command.CommandText = request.SqlText;
            command.CommandTimeout = request.TimeoutSeconds ?? 30;

            if (request.Parameters != null)
            {
                command.Parameters.AddRange(SqlParameterHelper.ToSqlParameters(request.Parameters));
            }

            await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            var rows = new List<Dictionary<string, object?>>();

            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var val = reader.GetValue(i);
                    row[reader.GetName(i)] = val == DBNull.Value ? null : val;
                }
                rows.Add(row);
            }

            watch.Stop();
            return new QueryResponse { Rows = rows, TotalCount = rows.Count, ExecutionTimeMs = watch.ElapsedMilliseconds };
        }
        catch (Exception ex) when (ex is not DatabaseException)
        {
            throw new ExecutionException($"Query execution failed: {ex.Message}", ex);
        }
    }

    public async Task<T?> ExecuteScalarAsync<T>(ExecuteQueryRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateExecuteQueryRequest(request);

        try
        {
            await using var connection = await _connectionFactory.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            await using var command = connection.CreateCommand();
            command.CommandText = request.SqlText;
            command.CommandTimeout = request.TimeoutSeconds ?? 30;

            if (request.Parameters != null)
            {
                command.Parameters.AddRange(SqlParameterHelper.ToSqlParameters(request.Parameters));
            }

            var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
            if (result == null || result == DBNull.Value) return default;

            return (T)Convert.ChangeType(result, typeof(T));
        }
        catch (Exception ex) when (ex is not DatabaseException)
        {
            throw new ExecutionException($"Scalar query execution failed: {ex.Message}", ex);
        }
    }
}
