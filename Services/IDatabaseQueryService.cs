namespace DataSharedLib.Services;

using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;

public interface IDatabaseQueryService
{
    Task<QueryResponse> ExecuteQueryAsync(ExecuteQueryRequest request, CancellationToken cancellationToken = default);
    Task<T?> ExecuteScalarAsync<T>(ExecuteQueryRequest request, CancellationToken cancellationToken = default);
}
