using DataSharedLib.Models.Requests;

namespace DataSharedLib.Validation;

public interface IRequestValidator
{
    void ValidateExecuteQuery(ExecuteQueryRequest request);
    void ValidateRead(ReadRequest request);
    void ValidateCreate(CreateRequest request);
    void ValidateBulkCreate(BulkCreateRequest request);
    void ValidateUpdate(UpdateRequest request);
    void ValidateDelete(DeleteRequest request);
    void ValidateStoredProcedure(StoredProcedureRequest request);
}
