using Microsoft.EntityFrameworkCore;
using StudentManagement.DTOs;

namespace StudentManagement.Domain.Repositories
{
    public static class PaginationExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query, int pageNumber, int pageSize)
        {
            int totalCount = await query.CountAsync();

            List<T> items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}