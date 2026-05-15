using MyFirstAPI.Models;

namespace MyFirstAPI.Services
{
    public interface IStudentService
    {
        public ServiceResponse<List<StudentResponseDTO>> GetAllStudents();
        public ServiceResponse<StudentResponseDTO> GetStudentById(int id);
        public ServiceResponse<Students> AddStudent(Students std);
        public ServiceResponse<Students> RemoveStudent(int id);
        public ServiceResponse<Students> UpdateStudent(int id, Students student);
        public ServiceResponse<List<StudentResponseDTO>> GetStudentByName(string name);

    
    }
}