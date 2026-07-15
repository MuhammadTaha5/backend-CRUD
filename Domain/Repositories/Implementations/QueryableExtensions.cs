
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using StudentManagement.DTOs;
using StudentManagement.Helper.Enums;
using StudentManagement.Helper.Validations;

namespace StudentManagement.Domain.Repositories
{
    public static class QueryableExtensions<T>
    {
        /// <summary>
        /// This takes query and filters to apply, and build the query with the help of 
        /// property name, property value and operator
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filters">list of filter dictionary having filter name, operator, value</param>
        /// <param name="FilterableProperties">List of all properties on which filtering can be done</param>
        /// <returns> <see cref="{IQueryable query, Error}"/>to fetch result from Db </returns>
        /// <exception cref="InvalidOperationException">thrown if operator doesnt matches</exception>
        public static (IQueryable<T> Query, string? Error) ApplyFilters(
            IQueryable<T> query, List<FilterCriteria>? filters,string[] FilterableProperties)
        {
            if (filters == null || filters.Count == 0)
                return (query, null);

            var clauses = new List<string>();
            var values = new List<object>();

            foreach (var filter in filters)
            {
                //Checks the filter name, operator, value is fine
                var result = FilterValidator.Validate<T>(filter, FilterableProperties);

                if (!result.IsValid)
                    return (query, result.Error);

                var propName = result.Property!.Name;
                var idx = values.Count;
                //develops the query based on filter operator
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
        /// <summary>
        /// Add the apply filter section to query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sortBy">the atribute to defind on which to sort</param>
        /// <param name="desc">describes the direction of sort(true, false)</param>
        /// <param name="SortableProperties">List of allowed sortable properties</param>
        /// <returns> <see cref="{IQueryable{T}}"/>with the string attached</returns>
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