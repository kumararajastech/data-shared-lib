using System.Diagnostics;
using System.Text;
using DataSharedLib.Connection;
using DataSharedLib.Exceptions;
using DataSharedLib.Helpers;
using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;
using DataSharedLib.Validation;
using Microsoft.Data.SqlClient;

namespace DataSharedLib.Services;

public class DatabaseCrudService : IDatabaseCrudService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly IRequestValidator _validator;

    public DatabaseCrudService(IDatabaseConnectionFactory connectionFactory, IRequestValidator validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<QueryResponse<IDictionary<string, object?>>> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateRead(request);
        var sw = Stopwatch.StartNew();

        var cols = (request.SelectColumns != null && request.SelectColumns.Any()) 
            ? string.Join(", ", request.SelectColumns) 
            : "*";

        var sb = new StringBuilder($"SELECT {cols} FROM {request.TableName}");
        if (!string.IsNullOrWhiteSpace(request.WhereClause))
        {
            sb.Append($" WHERE {request.WhereClause}");
        }

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            sb.Append($" ORDER BY {request.OrderBy}");
            if (request.PageNumber.HasValue && request.PageSize.HasValue)
            {
                int offset = (request.PageNumber.Value - 1) * request.PageSize.Value;
                sb.Append($" OFFSET {offset} ROWS FETCH NEXT {request.PageSize.Value} ROWS ONLY");
            }
        }

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = sb.ToString();
        command.Parameters.AddRange(SqlParameterHelper.CreateParameters(request.Parameters));

        var list = new List<IDictionary<string, object?>>();
        try
        {
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var dict = new Dictionary<string, object?>(reader.FieldCount);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dict[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                list.Add(dict);
            }
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"ReadAsync operation failed: {ex.Message}", command.CommandText, ex, ex.Number);
        }

        sw.Stop();
        return new QueryResponse<IDictionary<string, object?>>
        {
            Records = list,
            TotalCount = list.Count,
            ElapsedTime = sw.Elapsed
        };
    }

    public async Task<OperationResponse> CreateAsync(CreateRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateCreate(request);
        var sw = Stopwatch.StartNew();

        var colNames = string.Join(", ", request.ColumnValues.Keys);
        var paramNames = string.Join(", ", request.ColumnValues.Keys.Select(k => "@" + k));

        var sql = $"INSERT INTO {request.TableName} ({colNames}) VALUES ({paramNames});";
        if (request.ReturnIdentity)
        {
            sql += " SELECT SCOPE_IDENTITY();";
        }

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(SqlParameterHelper.CreateParameters(request.ColumnValues));

        object? identityValue = null;
        int rows = 0;
        try
        {
            if (request.ReturnIdentity)
            {
                identityValue = await command.ExecuteScalarAsync(cancellationToken);
                rows = identityValue != null && identityValue != DBNull.Value ? 1 : 0;
            }
            else
            {
                rows = await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"CreateAsync failed: {ex.Message}", sql, ex, ex.Number);
        }

        sw.Stop();
        return new OperationResponse
        {
            IsSuccess = true,
            RowsAffected = rows,
            PrimaryKeyId = identityValue,
            ElapsedTime = sw.Elapsed
        };
    }

    public async Task<OperationResponse> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateUpdate(request);
        var sw = Stopwatch.StartNew();

        var setClauses = string.Join(", ", request.ColumnValues.Keys.Select(k => $"{k} = @val_{k}"));
        var sql = $"UPDATE {request.TableName} SET {setClauses} WHERE {request.WhereClause};";

        var mergedParams = new Dictionary<string, object?>();
        foreach (var kvp in request.ColumnValues)
        {
            mergedParams[$"val_{kvp.Key}"] = kvp.Value;
        }
        foreach (var kvp in request.WhereParameters)
        {
            mergedParams[kvp.Key] = kvp.Value;
        }

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(SqlParameterHelper.CreateParameters(mergedParams));

        int rows;
        try
        {
            rows = await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"UpdateAsync failed: {ex.Message}", sql, ex, ex.Number);
        }

        sw.Stop();
        return new OperationResponse
        {
            IsSuccess = true,
            RowsAffected = rows,
            ElapsedTime = sw.Elapsed
        };
    }

    public async Task<OperationResponse> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateDelete(request);
        var sw = Stopwatch.StartNew();

        var sql = $"DELETE FROM {request.TableName} WHERE {request.WhereClause};";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(SqlParameterHelper.CreateParameters(request.Parameters));

        int rows;
        try
        {
            rows = await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"DeleteAsync failed: {ex.Message}", sql, ex, ex.Number);
        }

        sw.Stop();
        return new OperationResponse
        {
            IsSuccess = true,
            RowsAffected = rows,
            ElapsedTime = sw.Elapsed
        };
    }
}
