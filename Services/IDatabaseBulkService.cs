using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;

namespace DataSharedLib.Services;

public interface IDatabaseBulkService
{
    Task<OperationResponse> BulkInsertAsync(BulkCreateRequest request, CancellationToken cancellationToken = default);
    Task<OperationResponse> BulkInsertAsync<T>(string destinationTableName, IEnumerable<T> items, int batchSize = 5000, CancellationToken cancellationToken = default);
}
