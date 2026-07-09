using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models;
using StudentManagement.Domain.Repositories;
using StudentManagement.DTOs;

namespace StudentManagement.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbTable;
        protected  virtual string[] SearchableProperties { get; }
        protected virtual string[] SortableProperties { get; }
        protected virtual string DefaultSort => "Id";
        public Repository(ApplicationDbContext applicationDbContext)
        {
            _dbTable = applicationDbContext.Set<T>();
        }
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

            query = ApplySearch(query, queryParams.Search);
            query = ApplySort(query, queryParams.SortBy, queryParams.Desc);

            return await query.ToPagedResultAsync(queryParams.Page, queryParams.PageSize);
        }
        protected virtual IQueryable<T> ApplySearch(IQueryable<T> query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search) || SearchableProperties.Length == 0)
                return query;

            string predicate = string.Join(" || ",
                SearchableProperties.Select(p => $"{p}.ToLower().Contains(@0)"));

            return query.Where(predicate, search.ToLower());
        }
        protected virtual IQueryable<T> ApplySort(IQueryable<T> query, string? sortBy, bool desc)
        {
            string field = !string.IsNullOrWhiteSpace(sortBy) &&
                        SortableProperties.Contains(sortBy, StringComparer.OrdinalIgnoreCase)
                ? SortableProperties.First(p => p.Equals(sortBy, StringComparison.OrdinalIgnoreCase))
                : DefaultSort;

            return query.OrderBy(field + (desc ? " descending" : ""));
        }

    }
}