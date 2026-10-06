namespace DataSharedLib.Validation;

using DataSharedLib.Models.Requests;

public interface IRequestValidator
{
    void ValidateReadRequest(ReadRequest request);
    void ValidateCreateRequest(CreateRequest request);
    void ValidateUpdateRequest(UpdateRequest request);
    void ValidateDeleteRequest(DeleteRequest request);
    void ValidateExecuteQueryRequest(ExecuteQueryRequest request);
    void ValidateBulkCreateRequest(BulkCreateRequest request);
    void ValidateStoredProcedureRequest(StoredProcedureRequest request);
    string SanitizeIdentifier(string identifier);
}
