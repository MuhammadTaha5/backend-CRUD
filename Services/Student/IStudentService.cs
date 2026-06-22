using MyFirstAPI.Models;

namespace MyFirstAPI.Services
{
    public interface IStudentService
    {
        public ServiceResponse<List<StudentResponseDTO>> GetAllStudents();
        public ServiceResponse<StudentResponseDTO> GetStudentById(int id);
        public ServiceResponse<Student> AddStudent(Student std);
        public ServiceResponse<Student> RemoveStudent(int id);
        public ServiceResponse<Student> UpdateStudent(int id, Student student);
        public ServiceResponse<List<StudentResponseDTO>> GetStudentByName(string name);

    
    }
}