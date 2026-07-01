using MyFirstAPI.Data;
using MyFirstAPI.Models;
using StudentManagement.Repositories;

namespace StudentManagement
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IRepository<Student> StudentRepo { get; }
        
        public UnitOfWork(ApplicationDbContext applicationDb)
        {
            _dbContext = applicationDb;
            StudentRepo = new Repository<Student>(applicationDb);
        }
        public async Task<int> SaveAsync()
        => await _dbContext.SaveChangesAsync();

    public void Dispose() => _dbContext.Dispose();
    }
}