using MyFirstAPI.Models;
using StudentManagement.DTOs;

namespace MyFirstAPI.Services
{
    public interface IStudentService
    {
        public Task<List<StudentResponseDTO>> GetAllStudents();
        public Task<StudentResponseDTO> GetStudentById(int id);
        public Task<StudentResponseDTO> AddStudent(AddStudentDTO std);
        public Task<StudentResponseDTO> RemoveStudent(int id);
        public Task<StudentResponseDTO> UpdateStudent(int id, UpdateStudentDTO student);
        public Task<List<StudentResponseDTO>> GetStudentByName(string name);
        public Task<PagedResult<StudentResponseDTO>>GetStudentQuery(QueryParams p);

    
    }
}