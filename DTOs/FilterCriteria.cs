using System.ComponentModel.DataAnnotations;
using StudentManagement.Helper.Enums;

namespace StudentManagement.DTOs
{
    public class FilterCriteria
    {
        [Required]
        public string PropertyName { get; set; } = string.Empty;
        [Required]
        public string Value { get; set; } = string.Empty;
        [Required]
        public FilterOperator Operator { get; set; } = FilterOperator.Eq;
    }
}