namespace DataSharedLib.Services;

using System.Data;
using Microsoft.Data.SqlClient;

using DataSharedLib.Connection;
using DataSharedLib.Exceptions;
using DataSharedLib.Helpers;
using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;
using DataSharedLib.Validation;

public class DatabaseBulkService : IDatabaseBulkService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly IRequestValidator _validator;

    public DatabaseBulkService(IDatabaseConnectionFactory connectionFactory, IRequestValidator validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<OperationResponse> BulkInsertAsync(BulkCreateRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateBulkCreate(request);

        var startTime = DateTime.UtcNow;
        var sanitizedTable = _validator.SanitizeIdentifier(request.TableName);

        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        using var bulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = sanitizedTable,
            BatchSize = request.BatchSize,
            BulkCopyTimeout = request.TimeoutSeconds
        };

        if (request.ColumnMappings != null)
        {
            foreach (var mapping in request.ColumnMappings)
            {
                bulkCopy.ColumnMappings.Add(mapping.Key, mapping.Value);
            }
        }

        try
        {
            await bulkCopy.WriteToServerAsync(request.Data, cancellationToken);
            return new OperationResponse
            {
                Success = true,
                RowsAffected = request.Data.Rows.Count,
                ExecutionTime = DateTime.UtcNow - startTime
            };
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"Bulk insert operation failed: {ex.Message}", $"SqlBulkCopy -> {sanitizedTable}", ex.Number, ex);
        }
    }

    public async Task<OperationResponse> BulkInsertAsync<T>(string tableName, IEnumerable<T> items, int batchSize = 5000, CancellationToken cancellationToken = default)
    {
        var dataTable = DataTableHelper.ToDataTable(items);
        var request = new BulkCreateRequest
        {
            TableName = tableName,
            Data = dataTable,
            BatchSize = batchSize
        };
        return await BulkInsertAsync(request, cancellationToken);
    }
}
