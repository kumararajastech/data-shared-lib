using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;

namespace DataSharedLib.Services;

public interface IDatabaseQueryService
{
    Task<QueryResponse<IDictionary<string, object?>>> ExecuteQueryAsync(ExecuteQueryRequest request, CancellationToken cancellationToken = default);
    Task<T?> ExecuteScalarAsync<T>(string sql, IDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
}
