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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StudentService(IMapper mapper, ApplicationDbContext dbContext, IUnitOfWork unitOfWork)

        {
            _dbContext = dbContext;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<StudentResponseDTO>> GetStudentById(int id)
        {
            var record = await _unitOfWork.StudentRepo.GetByIdAsync(id);

            if (record != null)
            {
                var studentResponseDTO = _mapper.Map<StudentResponseDTO>(record);
                return ServiceResponse<StudentResponseDTO>.SuccessResponse(studentResponseDTO, "Record Found");
            }
            return ServiceResponse<StudentResponseDTO>.NotFoundResponse($"No record Found with ID {id}");
        }

        public async Task<ServiceResponse<List<StudentResponseDTO>>> GetAllStudents()
        {
            var allStudents = await _unitOfWork.StudentRepo.GetAllAsync();
            var studentResponseData = _mapper.Map<List<StudentResponseDTO>>(allStudents);

            return ServiceResponse<List<StudentResponseDTO>>.SuccessResponse(studentResponseData, "Students Retrieved Successfully");
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

            var addStudent = await _unitOfWork.StudentRepo.AddAsync(student);
            await _unitOfWork.SaveAsync();
            var response = _mapper.Map<StudentResponseDTO>(addStudent);
            return ServiceResponse<StudentResponseDTO>.SuccessResponse(response, "Record Added Successfully");

        }
        public async Task<ServiceResponse<StudentResponseDTO>> RemoveStudent(int id)
        {
            var getStudentRecord = await _unitOfWork.StudentRepo.GetByIdAsync(id);

            if (getStudentRecord == null)
            {
                return ServiceResponse<StudentResponseDTO>.NotFoundResponse($"No student record found with Id {id}");
            }
            await _unitOfWork.StudentRepo.DeleteAsync(getStudentRecord);

            int rowsAffected = await _unitOfWork.SaveAsync();
            if (rowsAffected > 0)
            {
                var responseDTO = _mapper.Map<StudentResponseDTO>(getStudentRecord);
                return ServiceResponse<StudentResponseDTO>.SuccessResponse(responseDTO, "Record removed");
            }
            return ServiceResponse<StudentResponseDTO>.FailResponse("No Record Removed");
        }
        public async Task<ServiceResponse<StudentResponseDTO>> UpdateStudent(int id, UpdateStudentDTO updatedStudent)
        {
            // Find existing student
            var existingStudent = await _dbContext.Students.FindAsync(id);

            // If not found
            if (existingStudent == null)
            {
                return ServiceResponse<StudentResponseDTO>.NotFoundResponse($"No User Found with Id {id}");
            }
            else
            {
                _mapper.Map(updatedStudent, existingStudent);
                await _unitOfWork.SaveAsync();
                var responseDTO = _mapper.Map<StudentResponseDTO>(existingStudent);
                return ServiceResponse<StudentResponseDTO>.SuccessResponse(responseDTO,"Record updated");

            }

        }

        public async Task<ServiceResponse<List<StudentResponseDTO>>> GetStudentByName(string name)
        {
            var getRecord = await _dbContext.Students.Where(s => s.Name.Contains(name)).ToListAsync();


            if (getRecord.Any())
            {
                var serviceResponseDTO = _mapper.Map<List<StudentResponseDTO>>(getRecord);
                
                return ServiceResponse<List<StudentResponseDTO>>.SuccessResponse(serviceResponseDTO,"Record Found");
            }
            
            return ServiceResponse<List<StudentResponseDTO>>.FailResponse("No record found");

        }
        public async Task<ServiceResponse<PagedResult<Student>>> GetStudentQuery(QueryParams p)
        {
            var result = await _unitOfWork.StudentRepo.GetQueryAsync(p);
            return ServiceResponse<PagedResult<Student>>.SuccessResponse(result, "Records found");

        }
    }
}