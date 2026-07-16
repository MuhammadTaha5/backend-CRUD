using MyFirstAPI.Data;
using MyFirstAPI.Models;
using StudentManagement.Domain.Repositories;
using StudentManagement.Repositories;

namespace StudentManagement
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IRepository<Student> StudentRepo { get; }
        
        public UnitOfWork(ApplicationDbContext applicationDb, IRepository<Student> StudentRepo)
        {
            _dbContext = applicationDb;
            this.StudentRepo = StudentRepo;
        }
        /// <summary>
        /// saves changes to database
        /// </summary>
        /// <returns> <see cref="{int}"/>states of database response</returns>
        public async Task<int> SaveAsync()
        => await _dbContext.SaveChangesAsync();

    public void Dispose() => _dbContext.Dispose();
    }
}