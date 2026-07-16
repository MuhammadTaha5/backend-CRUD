using MyFirstAPI.Models;
using StudentManagement.DTOs;

namespace MyFirstAPI.Services
{
    public interface IStudentService
    {
        public Task<ServiceResponse<List<StudentResponseDTO>>> GetAllStudents();
        public Task<ServiceResponse<StudentResponseDTO>> GetStudentById(int id);
        public Task<ServiceResponse<StudentResponseDTO>> AddStudent(AddStudentDTO std);
        public Task<ServiceResponse<StudentResponseDTO>> RemoveStudent(int id);
        public Task<ServiceResponse<StudentResponseDTO>> UpdateStudent(int id, UpdateStudentDTO student);
        public Task<ServiceResponse<List<StudentResponseDTO>>> GetStudentByName(string name);
        public Task<ServiceResponse<PagedResult<StudentResponseDTO>>>GetStudentQuery(QueryParams p);

    
    }
}