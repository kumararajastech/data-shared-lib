namespace DataSharedLib.Services;

using System.Text;
using DataSharedLib.Connection;
using DataSharedLib.Exceptions;
using DataSharedLib.Helpers;
using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;
using DataSharedLib.Validation;
using Microsoft.Data.SqlClient;

public class DatabaseCrudService : IDatabaseCrudService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly IRequestValidator _validator;

    public DatabaseCrudService(IDatabaseConnectionFactory connectionFactory, IRequestValidator validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<QueryResponse> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateReadRequest(request);
        var watch = System.Diagnostics.Stopwatch.StartNew();

        var cols = request.SelectColumns != null && request.SelectColumns.Any()
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
        }

        if (request.PageNumber.HasValue && request.PageSize.HasValue)
        {
            if (string.IsNullOrWhiteSpace(request.OrderBy))
            {
                sb.Append(" ORDER BY (SELECT NULL)");
            }
            var offset = (request.PageNumber.Value - 1) * request.PageSize.Value;
            sb.Append($" OFFSET {offset} ROWS FETCH NEXT {request.PageSize.Value} ROWS ONLY");
        }

        try
        {
            await using var connection = await _connectionFactory.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            await using var command = connection.CreateCommand();
            command.CommandText = sb.ToString();

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
            throw new ExecutionException($"Read operation failed: {ex.Message}", ex);
        }
    }

    public async Task<OperationResponse> CreateAsync(CreateRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateCreateRequest(request);
        var watch = System.Diagnostics.Stopwatch.StartNew();

        var sanitizedTable = _validator.SanitizeIdentifier(request.TableName);
        var keys = request.ColumnValues.Keys.Select(_validator.SanitizeIdentifier).ToList();
        var paramNames = keys.Select((k, idx) => $"@p{idx}").ToList();

        var sb = new StringBuilder($"INSERT INTO {sanitizedTable} ({string.Join(", ", keys)}) VALUES ({string.Join(", ", paramNames)});");
        if (request.ReturnIdentity)
        {
            sb.Append(" SELECT SCOPE_IDENTITY();");
        }

        try
        {
            await using var connection = await _connectionFactory.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            await using var command = connection.CreateCommand();
            command.CommandText = sb.ToString();

            int idx = 0;
            foreach (var kvp in request.ColumnValues)
            {
                command.Parameters.AddWithValue($"@p{idx++}", kvp.Value ?? DBNull.Value);
            }

            object? primaryKey = null;
            int rowsAffected;

            if (request.ReturnIdentity)
            {
                primaryKey = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                rowsAffected = primaryKey != null ? 1 : 0;
            }
            else
            {
                rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            watch.Stop();
            return new OperationResponse { Success = true, RowsAffected = rowsAffected, GeneratedPrimaryKey = primaryKey, ExecutionTimeMs = watch.ElapsedMilliseconds };
        }
        catch (Exception ex) when (ex is not DatabaseException)
        {
            throw new ExecutionException($"Create operation failed: {ex.Message}", ex);
        }
    }

    public async Task<OperationResponse> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateUpdateRequest(request);
        var watch = System.Diagnostics.Stopwatch.StartNew();

        var sanitizedTable = _validator.SanitizeIdentifier(request.TableName);
        var setClauses = new List<string>();
        var parameters = new List<SqlParameter>();

        int idx = 0;
        foreach (var kvp in request.ColumnValues)
        {
            var paramName = $"@set_{idx++}";
            setClauses.Add($"{_validator.SanitizeIdentifier(kvp.Key)} = {paramName}");
            parameters.Add(new SqlParameter(paramName, kvp.Value ?? DBNull.Value));
        }

        var sb = new StringBuilder($"UPDATE {sanitizedTable} SET {string.Join(", ", setClauses)} WHERE {request.WhereClause}");

        if (request.Parameters != null)
        {
            parameters.AddRange(SqlParameterHelper.ToSqlParameters(request.Parameters));
        }

        try
        {
            await using var connection = await _connectionFactory.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            await using var command = connection.CreateCommand();
            command.CommandText = sb.ToString();
            command.Parameters.AddRange(parameters.ToArray());

            int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            watch.Stop();

            return new OperationResponse { Success = true, RowsAffected = rowsAffected, ExecutionTimeMs = watch.ElapsedMilliseconds };
        }
        catch (Exception ex) when (ex is not DatabaseException)
        {
            throw new ExecutionException($"Update operation failed: {ex.Message}", ex);
        }
    }

    public async Task<OperationResponse> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateDeleteRequest(request);
        var watch = System.Diagnostics.Stopwatch.StartNew();

        var sanitizedTable = _validator.SanitizeIdentifier(request.TableName);
        var sb = new StringBuilder($"DELETE FROM {sanitizedTable} WHERE {request.WhereClause}");

        try
        {
            await using var connection = await _connectionFactory.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            await using var command = connection.CreateCommand();
            command.CommandText = sb.ToString();

            if (request.Parameters != null)
            {
                command.Parameters.AddRange(SqlParameterHelper.ToSqlParameters(request.Parameters));
            }

            int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            watch.Stop();

            return new OperationResponse { Success = true, RowsAffected = rowsAffected, ExecutionTimeMs = watch.ElapsedMilliseconds };
        }
        catch (Exception ex) when (ex is not DatabaseException)
        {
            throw new ExecutionException($"Delete operation failed: {ex.Message}", ex);
        }
    }
}
