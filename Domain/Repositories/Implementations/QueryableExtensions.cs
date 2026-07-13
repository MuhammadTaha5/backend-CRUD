
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using StudentManagement.DTOs;
using StudentManagement.Helper.Enums;
using StudentManagement.Helper.Validations;

namespace StudentManagement.Domain.Repositories
{
    public static class QueryableExtensions<T>
    {
        public static (IQueryable<T> Query, string? Error) ApplyFilters(
            IQueryable<T> query, List<FilterCriteria>? filters,string[] FilterableProperties)
        {
            if (filters == null || filters.Count == 0)
                return (query, null);

            var clauses = new List<string>();
            var values = new List<object>();

            foreach (var filter in filters)
            {
                var result = FilterValidator.Validate<T>(filter, FilterableProperties);

                if (!result.IsValid)
                    return (query, result.Error);

                var propName = result.Property!.Name;
                var idx = values.Count;

                string clause = filter.Operator switch
                {
                    FilterOperator.Eq => $"{propName} == @{idx}",
                    FilterOperator.Neq => $"{propName} != @{idx}",
                    FilterOperator.Gt => $"{propName} > @{idx}",
                    FilterOperator.Gte => $"{propName} >= @{idx}",
                    FilterOperator.Lt => $"{propName} < @{idx}",
                    FilterOperator.Lte => $"{propName} <= @{idx}",
                    FilterOperator.Contains => $"{propName}.ToLower().Contains(@{idx})",
                    _ => throw new InvalidOperationException("Unhandled operator")
                };

                object value = filter.Operator == FilterOperator.Contains
                    ? result.ConvertedValue!.ToString()!.ToLower()
                    : result.ConvertedValue!;

                clauses.Add(clause);
                values.Add(value);
            }

            string predicate = string.Join(" && ", clauses);
            return (query.Where(predicate, values.ToArray()), null);
        }
        
        public static IQueryable<T> ApplySort(IQueryable<T> query, string? sortBy, bool desc, string[] SortableProperties)
        {
            string field = !string.IsNullOrWhiteSpace(sortBy) &&
                        SortableProperties.Contains(sortBy, StringComparer.OrdinalIgnoreCase)
                ? SortableProperties.First(p => p.Equals(sortBy, StringComparison.OrdinalIgnoreCase))
                : "";

            return query.OrderBy(field + (desc ? " descending" : ""));
        }
    }
}