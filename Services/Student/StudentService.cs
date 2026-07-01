using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models;
using StudentManagement;
using StudentManagement.DTOs;
using StudentManagement.Repositories;
namespace MyFirstAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _dbContext;
        public List<Student> students;
        private readonly IRepository<Student> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StudentService(IMapper mapper, ApplicationDbContext dbContext, IRepository<Student> repo, IUnitOfWork unitOfWork)

        {
            _dbContext = dbContext;
            _mapper = mapper;
            _repository = repo;
            _unitOfWork = unitOfWork;
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
            //var record = await _dbContext.Students.FindAsync(id);
            var record = await _unitOfWork.StudentRepo.GetByIdAsync(id);


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
            //var allStudents = await _dbContext.Students.ToListAsync();
            var allStudents = await _unitOfWork.StudentRepo.GetAllAsync();
            var studentResponseDTOs = _mapper.Map<List<StudentResponseDTO>>(allStudents);

            serviceResponse.Data = studentResponseDTOs;
            serviceResponse.Message = "Students Retrieved Successfully";
            serviceResponse.success = true;

            return serviceResponse;
        }
        public async Task<ServiceResponse<StudentResponseDTO>> AddStudent(AddStudentDTO dto)
        {
            var student = new Student
            {
                Name = dto.Name,
                Age = dto.Age,
                Department = dto.Department,
                Gpa = dto.Gpa,
                section = dto.Section,
                Email = dto.Email
            };

            //_dbContext.Students.Add(student);
            //var record = await _dbContext.SaveChangesAsync();
            var addStudent = await _unitOfWork.StudentRepo.AddAsync(student);
            await _unitOfWork.SaveAsync();
            var response = _mapper.Map<StudentResponseDTO>(addStudent);
            return new ServiceResponse<StudentResponseDTO>
            {
                Data = response,
                Message = "New Record Added",
            };

        }
        public async Task<ServiceResponse<StudentResponseDTO>> RemoveStudent(int id)
        {
            var getStudentRecord = await _unitOfWork.StudentRepo.GetByIdAsync(id);

            if (getStudentRecord == null)
            {
                return new ServiceResponse<StudentResponseDTO>
                {
                    Message = $"No student record found with Id {id}",
                    success = false,
                    Data = null
                };
            }
            var deleteStudent = await _unitOfWork.StudentRepo.DeleteAsync(getStudentRecord);
            if (deleteStudent == null)
            {
                await _unitOfWork.SaveAsync();
            }

            var responseDTO = _mapper.Map<StudentResponseDTO>(getStudentRecord);

            return new ServiceResponse<StudentResponseDTO>
            {
                Data = responseDTO,
                Message = "Record removed",
                success = true
            };
        }
        public async Task<ServiceResponse<StudentResponseDTO>> UpdateStudent(int id, UpdateStudentDTO updatedStudent)
        {
            // Find existing student
            var existingStudent = await _dbContext.Students.FindAsync(id);

            // If not found
            if (existingStudent == null)
            {
                return new ServiceResponse<StudentResponseDTO>
                {
                    success = false,
                    Data = null,
                    Message = $"No User Found with Id {id}"
                };
            }
            else
            {
                _mapper.Map(updatedStudent, existingStudent);
                await _unitOfWork.SaveAsync();
                var responseDTO = _mapper.Map<StudentResponseDTO>(existingStudent);
                return new ServiceResponse<StudentResponseDTO>
                {
                    Message = "Record updated",
                    Data = responseDTO
                };

            }

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
        public async Task<ServiceResponse<PagedResult<Student>>> GetStudentQuery(QueryParams p)
        {
            var result = await _unitOfWork.StudentRepo.GetQueryAsync(p);

            return new ServiceResponse<PagedResult<Student>>
            {
                Data = result,
                success = true

            };

        }
    }
}