namespace StudentManagement.Repositories
{
    public interface IRepository<T> where T :class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task <T?> DeleteAsync(int id);
        Task <bool> ExistsAsyns(int id);

         
    }
}