using MyFirstAPI.Models;

namespace MyFirstAPI.Services
{
    public interface IStudentService
    {
        public Task<ServiceResponse<List<StudentResponseDTO>>> GetAllStudents();
        public Task<ServiceResponse<StudentResponseDTO>> GetStudentById(int id);
        public Task<ServiceResponse<Student>> AddStudent(Student std);
        public ServiceResponse<Student> RemoveStudent(int id);
        public ServiceResponse<Student> UpdateStudent(int id, Student student);
        public Task<ServiceResponse<List<StudentResponseDTO>>> GetStudentByName(string name);

    
    }
}