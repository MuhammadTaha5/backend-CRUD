using Microsoft.AspNetCore.Mvc;
using MyFirstAPI.Services;
using MyFirstAPI.Models;
namespace MyFirstAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private StudentService _getStudents;
        public StudentsController(StudentService getStudents)
        {
            _getStudents = getStudents;
        }
        [HttpGet]
        public ActionResult<List<Models.Students>> GetStudents()
        {
            var studentsRecord = _getStudents.GetAllStundents();

            return Ok(studentsRecord);
        }
        [HttpGet("{stdId}")]
        public ActionResult GetStudentById(int stdId)
        {
            var studentsRecord = _getStudents.GetStudentById(stdId);
            if (studentsRecord == null)
            {
                return NotFound("No record found");
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
                var studentRecord = _getStudents.GetStudentById(id);
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

            if (_getStudents.AddStudent(student) != null)
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
                return BadRequest("Invalid Student data");
            }
            var getStudentRecord = _getStudents.GetStudentById(studentId);
            if (getStudentRecord == null)
            {
                return NotFound($"No Student found with {studentId}");
            }
            if (getStudentRecord.Id != student.Id)
            {
                return BadRequest("Student Id does not match with the Record");
            }

            getStudentRecord.Name = student.Name;
            getStudentRecord.Id = student.Id;
            getStudentRecord.Gpa = student.Gpa;

            return NoContent();


        }
        [HttpDelete("{id}")]
        public ActionResult DeleteStudent(int id)
        {
            var deletedStudent = _getStudents.RemoveStudent(id);   
            if (deletedStudent == null)
            {
                return NotFound($"Student with ID {id} not found");
            }

            return Ok(new
            {
                Message = "Student deleted successfully",
                DeletedStudent = deletedStudent
            });
        }

    }
}