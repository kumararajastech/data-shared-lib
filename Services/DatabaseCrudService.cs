namespace DataSharedLib.Services;

using System.Text;
using Microsoft.Data.SqlClient;

using DataSharedLib.Connection;
using DataSharedLib.Exceptions;
using DataSharedLib.Helpers;
using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;
using DataSharedLib.Validation;

public class DatabaseCrudService : IDatabaseCrudService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly IRequestValidator _validator;

    public DatabaseCrudService(IDatabaseConnectionFactory connectionFactory, IRequestValidator validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<QueryResponse<IReadOnlyDictionary<string, object?>>> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateRead(request);

        var startTime = DateTime.UtcNow;
        var cols = (request.SelectColumns != null && request.SelectColumns.Count > 0)
            ? string.Join(", ", request.SelectColumns.Select(_validator.SanitizeIdentifier))
            : "*";

        var sb = new StringBuilder($"SELECT {cols} FROM {_validator.SanitizeIdentifier(request.TableName)}");

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

        string sql = sb.ToString();

        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        if (request.Parameters != null)
        {
            command.Parameters.AddRange(SqlParameterHelper.ToSqlParameters(request.Parameters));
        }

        try
        {
            var rows = new List<IReadOnlyDictionary<string, object?>>();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dict[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                rows.Add(dict);
            }

            return new QueryResponse<IReadOnlyDictionary<string, object?>>
            {
                Success = true,
                Data = rows,
                TotalRecords = rows.Count,
                ExecutionTime = DateTime.UtcNow - startTime
            };
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"Read operation failed: {ex.Message}", sql, ex.Number, ex);
        }
    }

    public async Task<OperationResponse> CreateAsync(CreateRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateCreate(request);

        var startTime = DateTime.UtcNow;
        var sanitizedTable = _validator.SanitizeIdentifier(request.TableName);

        var colNames = request.ColumnValues.Keys.Select(_validator.SanitizeIdentifier).ToList();
        var paramNames = request.ColumnValues.Keys.Select(k => $"@{k}").ToList();

        var sql = $"INSERT INTO {sanitizedTable} ({string.Join(", ", colNames)}) VALUES ({string.Join(", ", paramNames)});";
        if (request.ReturnIdentity)
        {
            sql += " SELECT SCOPE_IDENTITY();";
        }

        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(SqlParameterHelper.ToSqlParameters(request.ColumnValues));

        try
        {
            object? newId = null;
            int rowsAffected = 0;

            if (request.ReturnIdentity)
            {
                newId = await command.ExecuteScalarAsync(cancellationToken);
                rowsAffected = newId != null ? 1 : 0;
            }
            else
            {
                rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
            }

            return new OperationResponse
            {
                Success = true,
                RowsAffected = rowsAffected,
                GeneratedIdentifier = newId,
                ExecutionTime = DateTime.UtcNow - startTime
            };
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"Create operation failed: {ex.Message}", sql, ex.Number, ex);
        }
    }

    public async Task<OperationResponse> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateUpdate(request);

        var startTime = DateTime.UtcNow;
        var sanitizedTable = _validator.SanitizeIdentifier(request.TableName);

        var setClauses = request.ColumnValues.Keys.Select(k => $"{_validator.SanitizeIdentifier(k)} = @set_{k}").ToList();
        var sql = $"UPDATE {sanitizedTable} SET {string.Join(", ", setClauses)} WHERE {request.WhereClause};";

        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        foreach (var kvp in request.ColumnValues)
        {
            command.Parameters.AddWithValue($"@set_{kvp.Key}", kvp.Value ?? DBNull.Value);
        }

        if (request.WhereParameters != null)
        {
            command.Parameters.AddRange(SqlParameterHelper.ToSqlParameters(request.WhereParameters));
        }

        try
        {
            int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
            return new OperationResponse
            {
                Success = true,
                RowsAffected = rowsAffected,
                ExecutionTime = DateTime.UtcNow - startTime
            };
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"Update operation failed: {ex.Message}", sql, ex.Number, ex);
        }
    }

    public async Task<OperationResponse> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateDelete(request);

        var startTime = DateTime.UtcNow;
        var sanitizedTable = _validator.SanitizeIdentifier(request.TableName);
        var sql = $"DELETE FROM {sanitizedTable} WHERE {request.WhereClause};";

        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        if (request.Parameters != null)
        {
            command.Parameters.AddRange(SqlParameterHelper.ToSqlParameters(request.Parameters));
        }

        try
        {
            int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
            return new OperationResponse
            {
                Success = true,
                RowsAffected = rowsAffected,
                ExecutionTime = DateTime.UtcNow - startTime
            };
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"Delete operation failed: {ex.Message}", sql, ex.Number, ex);
        }
    }
}
