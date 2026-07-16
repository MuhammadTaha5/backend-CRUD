using System.Reflection;

namespace StudentManagement.Helper.Validations
{
    public class FilterValidationResult
    {
        public bool IsValid { get; set; }
        public string? Error { get; set; }
        public PropertyInfo? Property { get; set; }
        public object? ConvertedValue { get; set; }
    }
}