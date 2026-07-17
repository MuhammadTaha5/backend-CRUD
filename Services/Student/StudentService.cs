using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models;
using StudentManagement;
using StudentManagement.DTOs;
using StudentManagement.Exceptions;
using StudentManagement.Helper.Constants;
namespace MyFirstAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentService> _logger;
        public StudentService(IMapper mapper, ApplicationDbContext dbContext, IUnitOfWork unitOfWork, ILogger<StudentService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        /// <summary>
        /// Retrieves a student by Id and give generic response
        /// </summary>
        /// <param name="id">The Student Id</param>
        /// <returns> <see cref="ServiceResponse{StudentResponseDTO}"/> contains the student if found
        /// other give not found response. </returns>

        public async Task<StudentResponseDTO> GetStudentById(int id)
        {
            Student? record = await _unitOfWork.StudentRepo.GetByIdAsync(id);

            if (record == null)
            {
                throw new NotFoundException(ResponseMessages.NoRecordFound);
            }
            StudentResponseDTO studentResponseDTO = _mapper.Map<StudentResponseDTO>(record);
            return studentResponseDTO;
            
        }
        /// <summary>
        /// Retrieve all the students from the database and give back data in generic response
        /// </summary>
        /// <returns>
        /// <see cref="ServiceResponse{List{StudentResponseDTO}}"containing all students record
        /// </returns>
        public async Task<List<StudentResponseDTO>> GetAllStudents()
        {
            IEnumerable<Student> allStudents = await _unitOfWork.StudentRepo.GetAllAsync();
            List<StudentResponseDTO> studentResponseData = _mapper.Map<List<StudentResponseDTO>>(allStudents);

            return studentResponseData;
        }
        /// <summary>
        /// create a new student record
        /// </summary>
        /// <param name="dto">Student data to feed in</param>
        /// <returns> <see cref="ServiceResponse{StudentResponseDTO}"/> containing the newly created record.</returns>

        public async Task<StudentResponseDTO> AddStudent(AddStudentDTO dto)
        {
            try
            {
                Student student = _mapper.Map<Student>(dto);

                Student addStudent = await _unitOfWork.StudentRepo.AddAsync(student);
                int saved = await _unitOfWork.SaveAsync();
                StudentResponseDTO createdStudent = _mapper.Map<StudentResponseDTO>(addStudent);
                if (saved <= 0)
                {
                    //failed to affect/create any record
                    throw new ValidationException(new List<string>(){ResponseMessages.AddRecordFailed});
            
                }
                StudentResponseDTO resultDto = _mapper.Map<StudentResponseDTO>(student);
                return resultDto;

            }
            catch (DbUpdateException ex)
            {
                // e.g. FK violation, unique constraint, etc
                _logger.LogError(ex, "Database error while adding student");
                throw new ValidationException(new List<string>(){"A database error occurred while saving the student. Please check the submitted data."});
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding student");
                throw new ValidationException(new List<string>(){"An unexpected error occurred while processing your request."});
            }


        }
        /// <summary>
        /// Delete a student record from db
        /// </summary>
        /// <param name="id">The Id of Student to delete</param>
        /// <returns> <see cref="ServiceResponse{StudentResponseDTO}"/> containing the record which is deleted and not found reponse if not reccord not found with id</returns>
        public async Task<StudentResponseDTO> RemoveStudent(int id)
        {
            Student? getStudentRecord = await _unitOfWork.StudentRepo.GetByIdAsync(id);

            if (getStudentRecord == null)
            {
                throw new NotFoundException (ResponseMessages.NoRecordFound);
            }
            await _unitOfWork.StudentRepo.DeleteAsync(getStudentRecord);

            int rowsAffected = await _unitOfWork.SaveAsync();
            if (rowsAffected > 0)
            {
                StudentResponseDTO responseDTO = _mapper.Map<StudentResponseDTO>(getStudentRecord);
                return responseDTO;
            }
            throw new ValidationException(new List<string>(){ResponseMessages.NoRecordRemoved});
        }
        /// <summary>
        /// Updates the existing student details with id
        /// </summary>
        /// <param name="id">The Id of student to update</param>
        /// <param name="updatedStudent">New value of field to update.</param>
        /// <returns> <see cref="ServiceResponse{StudentResponseDTO}"/> contaning the updated student record and not found if no record found  </returns>
        public async Task<StudentResponseDTO> UpdateStudent(int id, UpdateStudentDTO updatedStudent)
        {
            // Find existing student
            Student? existingStudent = await _unitOfWork.StudentRepo.GetByIdAsync(id);

            // If not found
            if (existingStudent == null)
            {
                throw new NotFoundException(ResponseMessages.NoRecordFound);
            }
            else
            {
                try
                {
                    _mapper.Map(updatedStudent, existingStudent);
                    int responseCode = await _unitOfWork.SaveAsync();
                    StudentResponseDTO responseDTO = _mapper.Map<StudentResponseDTO>(existingStudent);
                    //if responsecode is less than 0 means no record updated
                    if (responseCode <= 0)
                    {
                        _logger.LogWarning("Update for student {Id} did not persist any changes", id);
                        throw new ValidationException(new List<string>(){ResponseMessages.UpdateFailed});
                    }
                    return responseDTO;

                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error while updating student {Id}", id);
                    throw new ValidationException(new List<string>(){"A database error occurred while updating the student. Please check the submitted data."});
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while updating student {Id}", id);
                    throw new ValidationException(new List<string>(){"An unexpected error occurred while processing your request."});
                    
                }


            }


        }
        /// <summary>
        /// Retrieve the students by names
        /// </summary>
        /// <param name="name">The name of student to find in database</param>
        /// <returns> <see cref="ServiceResponse{List{StudentResponseDTO}}"/> containing a list of students with matching letters.</returns>

        public async Task<List<StudentResponseDTO>> GetStudentByName(string name)
        {
            List<Student> getRecord = await _dbContext.Students.Where(s => s.Name.Contains(name)).ToListAsync();

            if (getRecord.Any())
            {
                List<StudentResponseDTO> serviceResponseDTO = _mapper.Map<List<StudentResponseDTO>>(getRecord);

                return serviceResponseDTO;
            }

            throw new NotFoundException(ResponseMessages.NoRecordFound);

        }
        /// <summary>
        /// returns a filtered, paginated, and sorted records based on given query parameter
        /// </summary>
        /// <param name="p">the search property, sorting acsending/descending, page and page size</param>
        /// <returns> <see cref="ServiceResponse{Student}"/> containing the matching paginated records</returns>
        public async Task<PagedResult<StudentResponseDTO>> GetStudentQuery(QueryParams p)
        {
            try
            {
                (List<Student> result,int totalCount) = await _unitOfWork.StudentRepo.GetQueryAsync(p);
                List<StudentResponseDTO> records = _mapper.Map<List<StudentResponseDTO>>(result);
        
                PagedResult<StudentResponseDTO> responseResult = new PagedResult<StudentResponseDTO>
                {
                    Records = records,
                    TotalCount = totalCount,
                    PageNumber = p.Page,
                    PageSize = p.PageSize
                };
                
                return responseResult;

            }
            catch (Exception e)
            {
                throw new ValidationException(new List<string>(){e.Message});
            }



        }
    }
}