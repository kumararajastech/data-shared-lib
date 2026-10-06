namespace DataSharedLib.Services;

using System.Data;
using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;

public interface IDatabaseBulkService
{
    Task<OperationResponse> BulkInsertAsync(BulkCreateRequest request, CancellationToken cancellationToken = default);
    Task<OperationResponse> BulkInsertAsync<T>(string tableName, IEnumerable<T> items, int batchSize = 5000, CancellationToken cancellationToken = default);
}
