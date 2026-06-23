using MyFirstAPI.Models;

namespace MyFirstAPI.Services
{
    public interface IStudentService
    {
        public Task<ServiceResponse<List<StudentResponseDTO>>> GetAllStudents();
        public Task<ServiceResponse<StudentResponseDTO>> GetStudentById(int id);
        public Task<ServiceResponse<Student>> AddStudent(Student std);
        public Task<ServiceResponse<StudentResponseDTO>> RemoveStudent(int id);
        public Task<ServiceResponse<StudentResponseDTO>> UpdateStudent(int id, UpdateStudentDTO student);
        public Task<ServiceResponse<List<StudentResponseDTO>>> GetStudentByName(string name);

    
    }
}