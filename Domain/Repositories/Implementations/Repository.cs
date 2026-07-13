using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using StudentManagement.Domain.Repositories;
using StudentManagement.DTOs;

namespace StudentManagement.Repositories
{
    public class Repository<T>(ApplicationDbContext applicationDbContext) : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbTable = applicationDbContext.Set<T>();
        protected virtual string[] FilterableProperties => Array.Empty<string>();
        protected virtual string[] SortableProperties { get; } = ["Id"];
        protected virtual string DefaultSort => "Id";

        protected virtual IQueryable<T> Table => _dbTable;
        public async Task<T> AddAsync(T entity)
        {
            await _dbTable.AddAsync(entity);
            return entity;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            if (entity == null) return false;
            _dbTable.Remove(entity);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            T? exists = await _dbTable.FindAsync(id);
            if (exists == null)
            {
                return false;
            }
            return true;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbTable.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbTable.FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbTable.Update(entity);
        }

        public virtual async Task<DTOs.PagedResult<T>> GetQueryAsync(QueryParams queryParams)
        {
            IQueryable<T> query = Table;

            var (filteredQuery, error) = QueryableExtensions<T>.ApplyFilters(query, queryParams.Filters, FilterableProperties);
            if (error != null)
                throw new ValidationException(error);

            query = QueryableExtensions<T>.ApplySort(filteredQuery, queryParams.SortBy, queryParams.Desc, SortableProperties);

            return await ToPagedResultAsync(query, queryParams.Page, queryParams.PageSize);
        }
        
        public static async Task<DTOs.PagedResult<T>> ToPagedResultAsync(IQueryable<T> query, int pageNumber, int pageSize)
        {
            int totalCount = await query.CountAsync();

            List<T> items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new DTOs.PagedResult<T>
            {
                Records = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

    }
}