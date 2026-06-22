using Microsoft.AspNetCore.Mvc;
using MyFirstAPI.Services;
using MyFirstAPI.Models;
namespace MyFirstAPI.Controllers
{
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
            var studentsRecord = await _studentService.GetAllStudents();

            return Ok(studentsRecord);
        }
        [HttpGet("{stdId}")]
        public async Task<ActionResult> GetStudentById(int stdId)
        {
            var studentsRecord = await _studentService.GetStudentById(stdId);
            if (!studentsRecord.success)
            {
                return NotFound(studentsRecord);
            }
            return Ok(studentsRecord);
        }
        [HttpGet("search")]
        public async Task<ActionResult<Models.Student>> GetStudentByName(string name, int id)
        {
            //Console.WriteLine($"Id: {id}");
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name is required");
            }

            //Console.WriteLine("Name: " + name);
            var studentRecord = await _studentService.GetStudentByName(name);
            Console.Write(studentRecord.Data);
            if (!studentRecord.success)
            {
                return NotFound(studentRecord);
            }
            return Ok(studentRecord);
        }
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<Student>>> AddStudent(AddStudentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = new Student
            {
                Name = dto.Name,
                Age = dto.Age,
                Department = dto.Department,
                Gpa = dto.Gpa,
                section = dto.Section,
                Email = dto.Email
            };

            var result = await _studentService.AddStudent(student);

            return CreatedAtAction(
                nameof(GetStudentById),
                new { stdId = result.Data.Id },
                result
            );
        }
        [HttpPut("{studentId}")]
        public ActionResult UpdateStudent(int studentId, Student student)
        {
            if (student == null)
            {
                return BadRequest("Student data is required.");
            }


            if (studentId != student.Id)
            {
                return BadRequest("Student ID mismatch.");
            }

            var updatedStudent = _studentService.UpdateStudent(studentId, student);


            if (!updatedStudent.success)
            {
                return NotFound($"No student found with ID {studentId}");
            }


            return NoContent();
        }
        [HttpDelete("{id}")]
        public ActionResult DeleteStudent(int id)
        {
            var deletedStudent = _studentService.RemoveStudent(id);
            if (!deletedStudent.success)
            {
                return NotFound(deletedStudent);
            }

            return Ok(new
            {
                Message = "Student deleted successfully",
                DeletedStudent = deletedStudent
            });
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