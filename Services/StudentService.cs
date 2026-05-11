using MyFirstAPI.Models;
namespace MyFirstAPI.Services
{
    public class StudentService :IStudentService
    {
        public List<Students> students;
        public StudentService()
        {
            students = new List<Students>
            {
                new Students
                {
                    Id = 132,
                    Name = "Muhammad Taha",
                    Gpa = 3.79

                },
                new Students
                {
                    Id = 108,
                    Name = "Muhammad Faisal",
                    Gpa = 3.08

                },
                new Students
                {
                    Id = 020,
                    Name = "Abeera",
                    Gpa = 3.44

                }
                
            };
        }

        public Students GetStudentById(int id)
        {
            var record = students.FirstOrDefault(s => s.Id == id);
            if (record!=null)
            {
                return record;
            }
            return null;
        }

        public List<Students> GetAllStundents()
        {
            return students;
        }
    }
}