using MyFirstAPI.Models;
using StudentManagement.DTOs;

namespace StudentManagement.Repositories
{
    public interface IRepository<T> where T :class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task <bool> DeleteAsync(T entity);
        Task <bool> ExistsAsync(int id);
        
        Task<PagedResult<T>> GetQueryAsync(QueryParams queryParams);

         
    }
}