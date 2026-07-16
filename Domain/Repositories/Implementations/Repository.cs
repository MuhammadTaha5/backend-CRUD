using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using StudentManagement.Attributes;
using StudentManagement.Domain.Repositories;
using StudentManagement.DTOs;

namespace StudentManagement.Repositories
{
    public class Repository<T>(ApplicationDbContext applicationDbContext) : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbTable = applicationDbContext.Set<T>();
        //protected virtual string[] FilterableProperties => Array.Empty<string>();
        //protected virtual string[] SortableProperties { get; } = ["Id"];
        protected virtual string DefaultSort => "Id";

        protected virtual IQueryable<T> Table => _dbTable;

        protected virtual string[] FilterableProperties =>
        typeof(T).GetProperties()
        .Where(p => p.IsDefined(typeof(FilterableAttribute)))
        .Select(p => p.Name)
        .ToArray();
        protected virtual string[] SortableProperties =>
        typeof(T).GetProperties()
        .Where(p => p.IsDefined(typeof(SortableAttribute)))
        .Select(p => p.Name)
        .ToArray();
        /// <summary>
        /// Adds a new entity to the underlying table's change tracker.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns> The added entity (with any generated values populated after <c>SaveAsync</c> is called) </returns>
        /// <remarks>
        /// Does not persist changes to the database — this only stages the insert.
        /// Call <c>IUnitOfWork.SaveAsync()</c> afterward to commit the transaction.
        /// </remarks>
        public async Task<T> AddAsync(T entity)
        {
            await _dbTable.AddAsync(entity);
            return entity;
        }
        /// <summary>
        /// Marks an entity for removal from the underlying table's change tracker.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns><c>true</c> if the entity was staged for removal; <c>false</c> if <paramref name="entity"/> was null.</returns>
        /// <remarks>
        /// Does not persist changes to the database — this only stages the delete.
        /// Call <c>IUnitOfWork.SaveAsync()</c> afterward to commit the transaction.
        /// </remarks>
        public async Task<bool> DeleteAsync(T entity)
        {
            if (entity == null) return false;
            _dbTable.Remove(entity);
            return true;
        }
        /// <summary>
        /// Checks if the entity exists in Database
        /// </summary>
        /// <param name="id">the id of entity</param>
        /// <returns>true if entity exist else false </returns>

        public async Task<bool> ExistsAsync(int id)
        {
            T? exists = await _dbTable.FindAsync(id);
            if (exists == null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Gets all records from database
        /// </summary>
        /// <returns> <see cref="{IEnumerable{T}}"/> all records</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbTable.ToListAsync();
        }
        /// <summary>
        /// Retrieves an entity by its primary key.
        /// </summary>
        /// <param name="id">The entity's primary key value.</param>
        /// <returns>The entity if found; otherwise <c>null</c>.</returns>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbTable.FindAsync(id);
        }
        /// <summary>
        /// Marks an entity as modified in the underlying table's change tracker.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        /// <remarks>
        /// Does not persist changes to the database — this only stages the update.
        /// Call <c>IUnitOfWork.SaveAsync()</c> afterward to commit the transaction.
        /// </remarks>
        public Task UpdateAsync(T entity)
        {
            _dbTable.Update(entity);
            return Task.CompletedTask;
        }
        /// <summary>
        /// Retrieves a filtered, sorted, and paginated list of entities based on the given query parameters.
        /// </summary>
        /// <param name="queryParams">Pagination, filter, and sort parameters from the client.</param>
        /// <returns>
        /// A tuple containing the page of matching items and the total count of items
        /// across all pages (before pagination is applied), used for building pagination metadata.
        /// </returns>
        /// <exception cref="ValidationException">
        /// Thrown if a filter references a property not in <see cref="FilterableProperties"/>,
        /// or uses an operator unsupported for that property's type.
        /// </exception>

        public virtual async Task<(List<T> Items, int TotalCount)>GetQueryAsync(QueryParams queryParams)
        {
            IQueryable<T> query = Table;

            var (filteredQuery, error) = QueryableExtensions<T>.ApplyFilters(query, queryParams.Filters, FilterableProperties);
            if (error != null)
                throw new ValidationException(error);

            query = QueryableExtensions<T>.ApplySort(filteredQuery, queryParams.SortBy, queryParams.Desc, SortableProperties);

            int totalCount = await query.CountAsync();

            List<T> items = await query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        
    }
}