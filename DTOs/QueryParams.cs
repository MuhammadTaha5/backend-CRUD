using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTOs
{
    public class QueryParams
    {
        public List<FilterCriteria>? Filters { get; set; }
        public string SortBy { get; set; } = "Id";
        public bool Desc { get; set; } = false;
        public int Page { get; set; } = 1;
        [Range(1, 20)]
        public int PageSize { get; set; } = 10;
        
    }
}