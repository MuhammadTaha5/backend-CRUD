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
        public ActionResult<List<Models.Student>> GetStudents()
        {
            var studentsRecord = _studentService.GetAllStudents();

            return Ok(studentsRecord);
        }
        [HttpGet("{stdId}")]
        public ActionResult GetStudentById(int stdId)
        {
            var studentsRecord = _studentService.GetStudentById(stdId);
            if (!studentsRecord.success)
            {
                return NotFound(studentsRecord);
            }
            return Ok(studentsRecord);
        }
        [HttpGet("search")]
        public ActionResult<Models.Student> GetStudentByName(string name, int id)
        {
            //Console.WriteLine($"Id: {id}");
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name is required");
            }
            
            //Console.WriteLine("Name: " + name);
            var studentRecord = _studentService.GetStudentByName(name);
            Console.Write(studentRecord.Data);
            if (!studentRecord.success)
            {
                return NotFound($"No User Found {name}");
            }
            return Ok(studentRecord);
        }
        [HttpPost]
        public ActionResult<Student> AddStudent(Student student)
        {
            if (student == null)
            {
                return BadRequest("Student object is null");
            }

            if (_studentService.AddStudent(student) != null)
            {
                return CreatedAtAction(nameof(GetStudentById), new { stdId = student.Id }, student);
            }
            return BadRequest("Failed to add Student");


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
            return Ok( new
            {
                service1= _service1.GetOperationId(),
                service2= _service2.GetOperationId(),
                }

            );
        }
        

    }
}