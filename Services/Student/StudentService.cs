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
        /// <summary>
        /// Retrieves a student by Id and give generic response
        /// </summary>
        /// <param name="id">The Student Id</param>
        /// <returns> <see cref="ServiceResponse{StudentResponseDTO}"/> contains the student if found
        /// other give not found response. </returns>

        public async Task<ServiceResponse<StudentResponseDTO>> GetStudentById(int id)
        {
            Student? record = await _unitOfWork.StudentRepo.GetByIdAsync(id);

            if (record != null)
            {
                StudentResponseDTO studentResponseDTO = _mapper.Map<StudentResponseDTO>(record);
                return ServiceResponse<StudentResponseDTO>.SuccessResponse(studentResponseDTO, "Record Found");
            }
            return ServiceResponse<StudentResponseDTO>.NotFoundResponse($"No record Found with ID {id}");
        }
        /// <summary>
        /// Retrieve all the students from the database and give back data in generic response
        /// </summary>
        /// <returns>
        /// <see cref="ServiceResponse{List{StudentResponseDTO}}"containing all students record
        /// </returns>
        public async Task<ServiceResponse<List<StudentResponseDTO>>> GetAllStudents()
        {
            IEnumerable<Student> allStudents = await _unitOfWork.StudentRepo.GetAllAsync();
            List<StudentResponseDTO> studentResponseData = _mapper.Map<List<StudentResponseDTO>>(allStudents);

            return ServiceResponse<List<StudentResponseDTO>>.SuccessResponse(studentResponseData, "Students Retrieved Successfully");
        }
        /// <summary>
        /// create a new student record
        /// </summary>
        /// <param name="dto">Student data to feed in</param>
        /// <returns> <see cref="ServiceResponse{StudentResponseDTO}"/> containing the newly created record.</returns>

        public async Task<ServiceResponse<StudentResponseDTO>> AddStudent(AddStudentDTO dto)
        {
            Student student = _mapper.Map<Student>(dto);

            Student addStudent = await _unitOfWork.StudentRepo.AddAsync(student);
            await _unitOfWork.SaveAsync();
            StudentResponseDTO createdStudent = _mapper.Map<StudentResponseDTO>(addStudent);
            return ServiceResponse<StudentResponseDTO>.SuccessResponse(createdStudent, "Record Added Successfully");

        }
        /// <summary>
        /// Delete a student record from db
        /// </summary>
        /// <param name="id">The Id of Student to delete</param>
        /// <returns> <see cref="ServiceResponse{StudentResponseDTO}"/> containing the record which is deleted and not found reponse if not reccord not found with id</returns>
        public async Task<ServiceResponse<StudentResponseDTO>> RemoveStudent(int id)
        {
            Student? getStudentRecord = await _unitOfWork.StudentRepo.GetByIdAsync(id);

            if (getStudentRecord == null)
            {
                return ServiceResponse<StudentResponseDTO>.NotFoundResponse($"No student record found with Id {id}");
            }
            await _unitOfWork.StudentRepo.DeleteAsync(getStudentRecord);

            int rowsAffected = await _unitOfWork.SaveAsync();
            if (rowsAffected > 0)
            {
                StudentResponseDTO responseDTO = _mapper.Map<StudentResponseDTO>(getStudentRecord);
                return ServiceResponse<StudentResponseDTO>.SuccessResponse(responseDTO, "Record removed");
            }
            return ServiceResponse<StudentResponseDTO>.FailResponse("No Record Removed");
        }
        /// <summary>
        /// Updates the existing student details with id
        /// </summary>
        /// <param name="id">The Id of student to update</param>
        /// <param name="updatedStudent">New value of field to update.</param>
        /// <returns> <see cref="ServiceResponse{StudentResponseDTO}"/> contaning the updated student record and not found if no record found  </returns>
        public async Task<ServiceResponse<StudentResponseDTO>> UpdateStudent(int id, UpdateStudentDTO updatedStudent)
        {
            // Find existing student
            Student? existingStudent = await _dbContext.Students.FindAsync(id);

            // If not found
            if (existingStudent == null)
            {
                return ServiceResponse<StudentResponseDTO>.NotFoundResponse($"No User Found with Id {id}");
            }
            else
            {
                _mapper.Map(updatedStudent, existingStudent);
                await _unitOfWork.SaveAsync();
                StudentResponseDTO responseDTO = _mapper.Map<StudentResponseDTO>(existingStudent);
                return ServiceResponse<StudentResponseDTO>.SuccessResponse(responseDTO,"Record updated");

            }

        }
        /// <summary>
        /// Retrieve the students by names
        /// </summary>
        /// <param name="name">The name of student to find in database</param>
        /// <returns> <see cref="ServiceResponse{List{StudentResponseDTO}}"/> containing a list of students with matching letters.</returns>

        public async Task<ServiceResponse<List<StudentResponseDTO>>> GetStudentByName(string name)
        {
            List<Student> getRecord = await _dbContext.Students.Where(s => s.Name.Contains(name)).ToListAsync();


            if (getRecord.Any())
            {
                List<StudentResponseDTO> serviceResponseDTO = _mapper.Map<List<StudentResponseDTO>>(getRecord);
                
                return ServiceResponse<List<StudentResponseDTO>>.SuccessResponse(serviceResponseDTO,"Record Found");
            }
            
            return ServiceResponse<List<StudentResponseDTO>>.FailResponse("No record found");

        }
        /// <summary>
        /// returns a filtered, paginated, and sorted records based on given query parameter
        /// </summary>
        /// <param name="p">the search property, sorting acsending/descending, page and page size</param>
        /// <returns> <see cref="ServiceResponse{Student}"/> containing the matching paginated records</returns>
        public async Task<ServiceResponse<PagedResult<Student>>> GetStudentQuery(QueryParams p)
        {
            PagedResult<Student> result = await _unitOfWork.StudentRepo.GetQueryAsync(p);
            return ServiceResponse<PagedResult<Student>>.SuccessResponse(result, "Records found");

        }
    }
}