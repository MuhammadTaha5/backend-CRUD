using Microsoft.AspNetCore.Mvc;
using MyFirstAPI.Services;
using MyFirstAPI.Models;
using Microsoft.AspNetCore.Authorization;
using StudentManagement.DTOs;
using StudentManagement.Constants;

namespace MyFirstAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private IStudentService _studentService;
        private readonly ILogService _service1;
        private readonly ILogService _service2;
        public StudentsController(IStudentService studentService, ILogService s1, ILogService s2)
        {
            _studentService = studentService;
            _service1 = s1;
            _service2 = s2;
        }
        /// <summary>
        /// Gets all students
        /// </summary>
        /// <returns>A lists of students wrapped in a <see cref="ServiceResponse{T}"/></returns>

        [HttpGet]
        public async Task<ActionResult<List<Student>>> GetStudents()
        {
            ServiceResponse<List<StudentResponseDTO>> studentsRecord = await _studentService.GetAllStudents();

            return Ok(studentsRecord);
        }
        /// <summary>
        /// Gets a single student by ID.
        /// </summary>
        /// <param name="stdId">The student's ID.</param>
        /// <returns>The student record if found, or 404 if no student exists with that ID.</returns>
        /// <response code="200">Student found and returned.</response>
        /// <response code="404">No student exists with the given ID.</response>
        [HttpGet("{stdId}")]
        public async Task<ActionResult> GetStudentById(int stdId)
        {
            ServiceResponse<StudentResponseDTO> studentsRecord = await _studentService.GetStudentById(stdId);
            if (!studentsRecord.success)
            {
                return NotFound(studentsRecord);
            }
            return Ok(studentsRecord);
        }
        /// <summary>
        /// Geta student records with matching names.
        /// </summary>
        /// <param name="name">The name of Student to find</param>
        /// <returns> <see cref="ServiceResponse{List{StudentResponseDTO}}"/>containing the list of students with matching names </returns>
        [HttpGet("search")]
        public async Task<ActionResult<Student>> GetStudentByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest(ServiceResponse<string>.FailResponse("Name is required", null));
            }
            ServiceResponse<List<StudentResponseDTO>> studentRecord = await _studentService.GetStudentByName(name);
            Console.Write(studentRecord.Data);
            if (!studentRecord.success)
            {
                return NotFound(studentRecord);
            }
            return Ok(studentRecord);
        }
        /// <summary>
        ///Create student record in database. Admin only endpoint.
        /// </summary>
        /// <param name="dto">The Fields of users</param>
        /// <returns> <see cref="ServiceResponse{StudentResponseDTO}"/> contains the record created in database.</returns>
        /// <response code="400">Request body failed validation.</response>
        /// <response code="401">Caller is not authenticated.</response>
        /// <response code="403">Caller is authenticated but not an Admin.</response>
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<ServiceResponse<StudentResponseDTO>>> AddStudent(AddStudentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            ServiceResponse<StudentResponseDTO> createStudent = await _studentService.AddStudent(dto);
            if (createStudent.success)
            {
                return Ok(createStudent);
            }
            return BadRequest(createStudent);
        }
        /// <summary>
        /// Updates the record of students with id, new fields. Admin only endpoint
        /// </summary>
        /// <param name="studentId">The ID of student to update</param>
        /// <param name="dto">The new field to update the existing record</param>
        /// <returns>The updated student record, or 404 if no student exists with the given ID.</returns>
        /// <response code="200">Student updated successfully.</response>
        /// <response code="400">Request body failed validation.</response>
        /// <response code="401">Caller is not authenticated.</response>
        /// <response code="403">Caller is authenticated but not an Admin.</response>
        /// <response code="404">No student exists with the given ID.</response>
        [HttpPut("{studentId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> UpdateStudent(int studentId, UpdateStudentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ServiceResponse<StudentResponseDTO> updatedStudent = await _studentService.UpdateStudent(studentId, dto);

            if (!updatedStudent.success)
            {
                return NotFound(updatedStudent);
            }
            return Ok(updatedStudent);
        }


        /// <summary>
        /// Delete the Student record with Id
        /// </summary>
        /// <param name="id">The Id of Student to delete</param>
        /// <returns>The Record of Student that is deleted or 404 if not record found with Id</returns>
        /// <response code="200">Student deleted successfully.</response>
        /// <response code="401">Caller is not authenticated.</response>
        /// <response code="403">Caller is authenticated but not an Admin.</response>
        /// <response code="404">No student exists with the given ID.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            ServiceResponse<StudentResponseDTO> deletedStudent = await _studentService.RemoveStudent(id);
            if (!deletedStudent.success)
            {
                return NotFound(deletedStudent);
            }

            return Ok(deletedStudent);
        }
        /// <summary>
        /// gets record based on query parameter
        /// </summary>
        /// <param name="queryParams">The Search property, filter, sorting</param>
        /// <returns>the filtered, sorted ascending/descending , and paginated records</returns>
        
        [HttpGet("query")]
        public async Task<ActionResult> GetStudentQuery([FromQuery] QueryParams queryParams)
        {
            ServiceResponse<PagedResult<StudentResponseDTO>> result = await _studentService.GetStudentQuery(queryParams);
            if(!result.success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        
        /*
        [HttpGet("query")]
        public IActionResult Query([FromQuery] QueryParams queryParams)
        {
            return Ok(queryParams);
        }
        */


        /// <summary>
        /// test the Singleton, Scoped, and transient behaviour
        /// </summary>
        /// <returns>Service response of Service 1 and service 2</returns>
        [HttpGet("/api/GetGuid")]
        public ActionResult GetGuid()
        {
            return Ok(new
            {
                service1 = _service1.GetOperationId(),
                service2 = _service2.GetOperationId(),
            }

            );
        }


    }
}