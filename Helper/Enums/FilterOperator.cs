using System.Text.Json.Serialization;

namespace StudentManagement.Helper.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    //Filter Operator enums
    public enum FilterOperator
    {
        Eq,
        Neq,
        Gt,
        Gte,
        Lt,
        Lte,
        Contains
    }
}