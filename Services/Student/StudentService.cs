using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models;
namespace MyFirstAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _dbContext;
        public List<Student> students;
        private readonly IMapper _mapper;
        public StudentService(IMapper mapper, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            students = new List<Student>
            {
                new Student
                {
                    Id = 132,
                    Name = "Muhammad Taha",
                    Gpa = 3.79,
                    Age = 21,
                    Department = "Computer Science",
                    Email = "taha.saeed339@gmail.com"

                },
                new Student
                {
                    Id = 108,
                    Name = "Muhammad Faisal",
                    Gpa = 3.08,
                    Age = 21,
                    Department = "Computer Science",
                    Email = "faisal@gmail.com"

                },
                new Student
                {
                    Id = 19,
                    Name = "Abdullah",
                    Gpa = 3.44,
                    Age = 21,
                    Department = "Computer Science",
                    Email = "abdullah@gmail.com"

                }

            };
        }

        public async Task<ServiceResponse<StudentResponseDTO>> GetStudentById(int id)
        {
            ServiceResponse<StudentResponseDTO> serviceResponse = new ServiceResponse<StudentResponseDTO>();

            // var record = students.FirstOrDefault(s => s.Id == id);
            var record = await _dbContext.Students.FindAsync(id);

            if (record != null)
            {
                var studentResponseDTO = _mapper.Map<StudentResponseDTO>(record);
                serviceResponse.Data = studentResponseDTO;
                serviceResponse.Message = "Record Found";
                return serviceResponse;
            }
            serviceResponse.Data = null;
            serviceResponse.Message = $"No record Found with ID {id}";
            serviceResponse.success = false;
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<StudentResponseDTO>>> GetAllStudents()
        {
            ServiceResponse<List<StudentResponseDTO>> serviceResponse = new ServiceResponse<List<StudentResponseDTO>>();
            var allStudents = await _dbContext.Students.ToListAsync();
            var studentResponseDTOs = _mapper.Map<List<StudentResponseDTO>>(allStudents);
            
            serviceResponse.Data = studentResponseDTOs;
            serviceResponse.Message = "Students Retrieved Successfully";
            serviceResponse.success = true;

            return serviceResponse;
        }
        public async Task<ServiceResponse<StudentResponseDTO>> AddStudent(AddStudentDTO dto)
        {
            ServiceResponse<StudentResponseDTO> serviceResponse = new ServiceResponse<StudentResponseDTO>();
            var student = new Student
            {
                Name = dto.Name,
                Age = dto.Age,
                Department = dto.Department,
                Gpa = dto.Gpa,
                section = dto.Section,
                Email = dto.Email
            };
            var response = _mapper.Map<StudentResponseDTO>(student);
            _dbContext.Students.Add(student);
            var record = await _dbContext.SaveChangesAsync();
            serviceResponse.Data = response;
            serviceResponse.Message = "New Record Added";
            return serviceResponse;
        }
        public async Task<ServiceResponse<StudentResponseDTO>> RemoveStudent(int id)
        {
            ServiceResponse<StudentResponseDTO> serviceResponse = new ServiceResponse<StudentResponseDTO>();

            var getStudentRecord = await _dbContext.Students.FindAsync(id);
            var responseDTOs = _mapper.Map<StudentResponseDTO>(getStudentRecord);
            
            
            if (getStudentRecord == null)
            {
                serviceResponse.Message = $"No student record found with Id {id}";
                serviceResponse.success = false;
                serviceResponse.Data = null;
                return serviceResponse;
            }
            if (getStudentRecord != null)
            {
                _dbContext.Students.Remove(getStudentRecord);
                await _dbContext.SaveChangesAsync();
            }

            serviceResponse.Data = responseDTOs;
            serviceResponse.Message = "Record removed";

            return serviceResponse;
        }
        public async Task<ServiceResponse<StudentResponseDTO>> UpdateStudent(int id, UpdateStudentDTO updatedStudent)
        {
            // Find existing student
            ServiceResponse<StudentResponseDTO> serviceResponse = new ServiceResponse<StudentResponseDTO>();


            var existingStudent = await _dbContext.Students.FindAsync(id);

            // If not found
            if (existingStudent==null)
            {
                serviceResponse.success = false;
                serviceResponse.Data = null;
                serviceResponse.Message = $"No User Found with Id {id}";
                return serviceResponse;
            }
            else
            {
                _mapper.Map(updatedStudent, existingStudent);
                await _dbContext.SaveChangesAsync();
                serviceResponse.Message = "Record updated";
                var responseDTO = _mapper.Map<StudentResponseDTO>(existingStudent);
                serviceResponse.Data = responseDTO;
                
            }
            
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<StudentResponseDTO>>> GetStudentByName(string name)
        {
            ServiceResponse<List<StudentResponseDTO>> serviceResponse = new ServiceResponse<List<StudentResponseDTO>>();
            #pragma warning disable CS8602 // Dereference of a possibly null reference.

            var getRecord = await _dbContext.Students.Where(s => s.Name.Contains(name)).ToListAsync();
            
            if (getRecord.Any())
            {
                var serviceResponseDTO = _mapper.Map<List<StudentResponseDTO>>(getRecord);
                serviceResponse.Data = serviceResponseDTO;
                serviceResponse.Message = "Record Found";
                serviceResponse.success = true;
                return serviceResponse;
            }
            serviceResponse.Data = null;
            serviceResponse.success = false;
            serviceResponse.Message = "No record found";
            return serviceResponse;

        }
    }
}