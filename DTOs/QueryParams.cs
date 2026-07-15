namespace StudentManagement.DTOs
{
    public class QueryParams
    {
        public string? Search { get; set; }
        public string SortBy { get; set; } = "Id";
        public bool Desc { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize {get; set;} = 10;
    }
}