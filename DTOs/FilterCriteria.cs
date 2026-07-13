using StudentManagement.Helper.Enums;

namespace StudentManagement.DTOs
{
    public class FilterCriteria
    {
        public string PropertyName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public FilterOperator Operator { get; set; } = FilterOperator.Eq;
    }
}