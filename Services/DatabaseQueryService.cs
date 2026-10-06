namespace DataSharedLib.Services;

using System.Data;
using Microsoft.Data.SqlClient;

using DataSharedLib.Connection;
using DataSharedLib.Exceptions;
using DataSharedLib.Helpers;
using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;
using DataSharedLib.Validation;

public class DatabaseQueryService : IDatabaseQueryService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly IRequestValidator _validator;

    public DatabaseQueryService(IDatabaseConnectionFactory connectionFactory, IRequestValidator validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<QueryResponse<T>> QueryAsync<T>(ExecuteQueryRequest request, Func<IReadOnlyDictionary<string, object?>, T> mapper, CancellationToken cancellationToken = default)
    {
        _validator.ValidateExecuteQuery(request);

        var startTime = DateTime.UtcNow;
        var items = new List<T>();

        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();

        command.CommandText = request.Sql;
        command.CommandTimeout = request.CommandTimeoutSeconds ?? 30;

        if (request.Parameters != null)
        {
            command.Parameters.AddRange(SqlParameterHelper.ToSqlParameters(request.Parameters));
        }

        try
        {
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                items.Add(mapper(row));
            }

            var elapsed = DateTime.UtcNow - startTime;
            return new QueryResponse<T>
            {
                Success = true,
                Data = items,
                TotalRecords = items.Count,
                ExecutionTime = elapsed
            };
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"Query execution failed: {ex.Message}", request.Sql, ex.Number, ex);
        }
    }

    public async Task<T?> ExecuteScalarAsync<T>(ExecuteQueryRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateExecuteQuery(request);

        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();

        command.CommandText = request.Sql;
        command.CommandTimeout = request.CommandTimeoutSeconds ?? 30;

        if (request.Parameters != null)
        {
            command.Parameters.AddRange(SqlParameterHelper.ToSqlParameters(request.Parameters));
        }

        try
        {
            var result = await command.ExecuteScalarAsync(cancellationToken);
            if (result == null || result == DBNull.Value) return default;
            return (T)Convert.ChangeType(result, typeof(T));
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"Scalar query failed: {ex.Message}", request.Sql, ex.Number, ex);
        }
    }
}
