namespace StudentManagement.Helper.Constants
{
    public static class ResponseMessages
{
    // Generic CRUD
    public const string AddedSuccessfully = "Record Added Successfully";
    public const string UpdatedSuccessfully = "Record updated";
    public const string UpdateFailed = "Failed to update record";
    public const string RemovedSuccessfully = "Record removed";
    public const string RecordFound = "Record Found";
    public const string RecordsFound = "Records found";

    public const string NoRecordFound = "No record found";
    public const string NoRecordRemoved = "No Record Removed";
    public const string AddRecordFailed = "Failed to Add Record";

    // Parameterized — const can't hold string interpolation with a runtime value,
    // so use static methods instead
    public static string NotFoundWithId(object id) => $"No record Found with ID {id}";

}
}