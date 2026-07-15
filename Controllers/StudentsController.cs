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
        [HttpGet]
        public async Task<ActionResult<List<Student>>> GetStudents()
        {
            ServiceResponse<List<StudentResponseDTO>> studentsRecord = await _studentService.GetAllStudents();

            return Ok(studentsRecord);
        }
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
        [HttpGet("search")]
        public async Task<ActionResult<Student>> GetStudentByName(string name, int id)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name is required");
            }
            ServiceResponse<List<StudentResponseDTO>> studentRecord = await _studentService.GetStudentByName(name);
            Console.Write(studentRecord.Data);
            if (!studentRecord.success)
            {
                return NotFound(studentRecord);
            }
            return Ok(studentRecord);
        }
        [HttpPost]
        [Authorize(Roles=Roles.Admin)]
        public async Task<ActionResult<ServiceResponse<StudentResponseDTO>>> AddStudent(AddStudentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ServiceResponse<StudentResponseDTO> result = await _studentService.AddStudent(dto);
            return Ok(result);
        }
        [HttpPut("{studentId}")]
        [Authorize(Roles=Roles.Admin)]
        public async Task <ActionResult> UpdateStudent(int studentId, UpdateStudentDTO dto)
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
        [HttpDelete("{id}")]
        [Authorize(Roles=Roles.Admin)]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            ServiceResponse<StudentResponseDTO> deletedStudent = await _studentService.RemoveStudent(id);
            if (!deletedStudent.success)
            {
                return NotFound(deletedStudent);
            }

            return Ok(deletedStudent);
        }
        [HttpGet("query")]
        public async Task<ActionResult> GetStudentQuery([FromQuery] QueryParams queryParams)
        {
            ServiceResponse<PagedResult<Student>> result = await _studentService.GetStudentQuery(queryParams);
            return Ok(result);
        }



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