using Microsoft.AspNetCore.Mvc;
using MyFirstAPI.Services;
using MyFirstAPI.Models;
namespace MyFirstAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private StudentService _studentService;
        public StudentsController(StudentService studentService)
        {
            _studentService = studentService;
        }
        [HttpGet]
        public ActionResult<List<Models.Students>> GetStudents()
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
        public ActionResult<Models.Students> GetStudentByName(string name, int id)
        {
            Console.WriteLine($"Id: {id}");
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name is required");
            }
            if (name != null)
            {
                Console.WriteLine("Name: " + name);
                var studentRecord = _studentService.GetStudentById(id);
                if (studentRecord == null)
                {
                    return NotFound($"No User Found {name}");
                }
                return Ok(studentRecord);

            }
            return NotFound($"No Record Found with {name}");
        }
        [HttpPost]
        public ActionResult<Students> AddStudent(Students student)
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
        public ActionResult UpdateStudent(int studentId, Students student)
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

    }
}