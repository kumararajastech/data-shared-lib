namespace DataSharedLib.Services;

using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;

public interface IDatabaseCrudService
{
    Task<QueryResponse> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default);
    Task<OperationResponse> CreateAsync(CreateRequest request, CancellationToken cancellationToken = default);
    Task<OperationResponse> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken = default);
    Task<OperationResponse> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default);
}
