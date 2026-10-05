using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;

namespace DataSharedLib.Services;

public interface IDatabaseCrudService
{
    Task<QueryResponse<IDictionary<string, object?>>> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default);
    Task<OperationResponse> CreateAsync(CreateRequest request, CancellationToken cancellationToken = default);
    Task<OperationResponse> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken = default);
    Task<OperationResponse> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default);
}
