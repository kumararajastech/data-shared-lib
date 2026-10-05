using System.Diagnostics;
using DataSharedLib.Connection;
using DataSharedLib.Exceptions;
using DataSharedLib.Helpers;
using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;
using DataSharedLib.Validation;
using Microsoft.Data.SqlClient;

namespace DataSharedLib.Services;

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
        var sw = Stopwatch.StartNew();

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, null)
        {
            DestinationTableName = request.DestinationTableName,
            BatchSize = request.BatchSize,
            BulkCopyTimeout = request.TimeoutSeconds
        };

        if (request.ColumnMappings != null && request.ColumnMappings.Count > 0)
        {
            foreach (var mapping in request.ColumnMappings)
            {
                bulkCopy.ColumnMappings.Add(mapping.Key, mapping.Value);
            }
        }

        try
        {
            await bulkCopy.WriteToServerAsync(request.DataTable, cancellationToken);
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"BulkInsertAsync failed for table {request.DestinationTableName}: {ex.Message}", request.DestinationTableName, ex, ex.Number);
        }

        sw.Stop();
        return new OperationResponse
        {
            IsSuccess = true,
            RowsAffected = request.DataTable.Rows.Count,
            ElapsedTime = sw.Elapsed
        };
    }

    public async Task<OperationResponse> BulkInsertAsync<T>(string destinationTableName, IEnumerable<T> items, int batchSize = 5000, CancellationToken cancellationToken = default)
    {
        var dataTable = DataTableHelper.ToDataTable(items, destinationTableName);
        var request = new BulkCreateRequest
        {
            DestinationTableName = destinationTableName,
            DataTable = dataTable,
            BatchSize = batchSize
        };
        return await BulkInsertAsync(request, cancellationToken);
    }
}
