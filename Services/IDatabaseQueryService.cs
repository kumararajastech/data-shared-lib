namespace DataSharedLib.Services;

using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;

public interface IDatabaseQueryService
{
    Task<QueryResponse<T>> QueryAsync<T>(ExecuteQueryRequest request, Func<IReadOnlyDictionary<string, object?>, T> mapper, CancellationToken cancellationToken = default);
    Task<T?> ExecuteScalarAsync<T>(ExecuteQueryRequest request, CancellationToken cancellationToken = default);
}
