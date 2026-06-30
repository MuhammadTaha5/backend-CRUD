using MyFirstAPI.Models;
using MyFirstAPI.Services;
using StudentManagement.Repositories;

namespace StudentManagement
{
    public interface IUnitOfWork: IDisposable
    {
        public IRepository<Student> StudentRepo { get; }
        Task<int> SaveAsync();
        
    }
}