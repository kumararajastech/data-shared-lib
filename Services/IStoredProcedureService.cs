using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;

namespace DataSharedLib.Services;

public interface IStoredProcedureService
{
    Task<StoredProcedureResponse> ExecuteStoredProcedureAsync(StoredProcedureRequest request, CancellationToken cancellationToken = default);
}
