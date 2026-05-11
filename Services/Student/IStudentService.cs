using MyFirstAPI.Models;

namespace MyFirstAPI.Services
{
    public interface IStudentService
    {
        public List<Students> GetAllStundents();
        public Students GetStudentById(int id);
        public Students AddStudent(Students std);
        public Students RemoveStudent(int id);
        public Students UpdateStudent(int id, Students student);

    
    }
}