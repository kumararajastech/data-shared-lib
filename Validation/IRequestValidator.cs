namespace DataSharedLib.Validation;

using DataSharedLib.Models.Requests;

public interface IRequestValidator
{
    string SanitizeIdentifier(string identifier);
    void ValidateRead(ReadRequest request);
    void ValidateCreate(CreateRequest request);
    void ValidateUpdate(UpdateRequest request);
    void ValidateDelete(DeleteRequest request);
    void ValidateBulkCreate(BulkCreateRequest request);
    void ValidateExecuteQuery(ExecuteQueryRequest request);
    void ValidateStoredProcedure(StoredProcedureRequest request);
}
