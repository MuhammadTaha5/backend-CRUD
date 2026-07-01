using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models;
using StudentManagement.DTOs;

namespace StudentManagement.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbTable;
        public Repository(ApplicationDbContext applicationDbContext)
        {
            _dbTable = applicationDbContext.Set<T>();
        }
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
            var exists = await _dbTable.FindAsync(id);
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
        public virtual async Task<PagedResult<T>> GetQueryAsync(QueryParams queryParams)
        {
            var query = _dbTable.AsQueryable();

            var total = await query.CountAsync();
            var items = await query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();
            Console.WriteLine("Hellohere");
            return new PagedResult<T>
            {
                Items = items,
                TotalCount = total,
                PageNumber = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

    }
}