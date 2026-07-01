using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models;

namespace StudentManagement.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly DbSet<T> _dbTable;
        public Repository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _dbTable = applicationDbContext.Set<T>();
        }
        public async Task<T> AddAsync(T entity)
        {
            await _dbTable.AddAsync(entity);
            return entity;
        }

        public async Task<T?> DeleteAsync(T entity)
        {
            if(entity!=null)
            {
                _dbTable.Remove(entity);
                return entity;
            }
            return null;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var exists = await _dbTable.FindAsync(id);
            if(exists==null)
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

    }
}