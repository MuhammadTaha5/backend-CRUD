using MyFirstAPI.Models;

namespace MyFirstAPI.Services
{
    public interface IStudentService
    {
        public List<Students> GetAllStundents();
        public Students GetStudentById(int id);

    
    }
}