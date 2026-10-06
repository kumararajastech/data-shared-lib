namespace DataSharedLib.Services;

using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;

public interface IStoredProcedureService
{
    Task<StoredProcedureResponse> ExecuteAsync(StoredProcedureRequest request, CancellationToken cancellationToken = default);
}
