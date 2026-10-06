namespace DataSharedLib.Services;

using System.Data;
using DataSharedLib.Connection;
using DataSharedLib.Exceptions;
using DataSharedLib.Helpers;
using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;
using DataSharedLib.Validation;
using Microsoft.Data.SqlClient;

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
        _validator.ValidateBulkCreateRequest(request);
        var watch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await using var connection = await _connectionFactory.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, null)
            {
                DestinationTableName = _validator.SanitizeIdentifier(request.TableName),
                BatchSize = request.BatchSize,
                BulkCopyTimeout = request.TimeoutSeconds ?? 60
            };

            if (request.ColumnMappings != null)
            {
                foreach (var map in request.ColumnMappings)
                {
                    bulkCopy.ColumnMappings.Add(map.Key, map.Value);
                }
            }

            await bulkCopy.WriteToServerAsync(request.DataTable, cancellationToken).ConfigureAwait(false);
            watch.Stop();

            return new OperationResponse { Success = true, RowsAffected = request.DataTable.Rows.Count, ExecutionTimeMs = watch.ElapsedMilliseconds };
        }
        catch (Exception ex) when (ex is not DatabaseException)
        {
            throw new ExecutionException($"Bulk insert operation failed: {ex.Message}", ex);
        }
    }

    public async Task<OperationResponse> BulkInsertAsync<T>(string tableName, IEnumerable<T> items, int batchSize = 5000, CancellationToken cancellationToken = default) where T : class
    {
        var dataTable = DataTableHelper.ToDataTable(items);
        var request = new BulkCreateRequest
        {
            TableName = tableName,
            DataTable = dataTable,
            BatchSize = batchSize
        };
        return await BulkInsertAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
