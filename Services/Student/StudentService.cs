using MyFirstAPI.Models;
namespace MyFirstAPI.Services
{
    public class StudentService : IStudentService
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
                    Id = 19,
                    Name = "Abdullah",
                    Gpa = 3.44

                }

            };
        }

        public ServiceResponse<Students> GetStudentById(int id)
        {
            ServiceResponse<Students> serviceResponse = new ServiceResponse<Students>();

            var record = students.FirstOrDefault(s => s.Id == id);
            if (record != null)
            {
                serviceResponse.Data = record;
                serviceResponse.Message = "Record Found";
                return serviceResponse;
            }
            serviceResponse.Data = null;
            serviceResponse.Message = $"No record Found with ID {id}";
            serviceResponse.success = false;
            return serviceResponse;
        }

        public ServiceResponse<List<Students>> GetAllStundents()
        {
            ServiceResponse<List<Students>> serviceResponse = new ServiceResponse<List<Students>>();
            serviceResponse.Data = students;

            return serviceResponse;
        }
        public ServiceResponse<Students> AddStudent(Students std)
        {
            ServiceResponse<Students> serviceResponse = new ServiceResponse<Students>();

            students.Add(std);
            serviceResponse.Data = std;
            serviceResponse.Message = "New Record Added";
            return serviceResponse;
        }
        public ServiceResponse<Students> RemoveStudent(int id)
        {
            ServiceResponse<Students> serviceResponse = new ServiceResponse<Students>();

            var getStudentRecord = GetStudentById(id);
            if (getStudentRecord == null)
            {
                serviceResponse.Message=$"No student record found with Id {id}";
                serviceResponse.success = false;

            }
            students.Remove(getStudentRecord.Data);
            serviceResponse.Data=getStudentRecord.Data;
            serviceResponse.Message= "Record removed";

            return serviceResponse;
        }
        public ServiceResponse<Students> UpdateStudent(int id, Students updatedStudent)
        {
            // Find existing student
            ServiceResponse<Students> serviceResponse = new ServiceResponse<Students>();

            var existingStudent = GetStudentById(id).Data;

            // If not found
            if (existingStudent == null)
            {
                serviceResponse.success = false;
                serviceResponse.Data = null;
                serviceResponse.Message = $"No User Found with Id {id}";
                return serviceResponse;
            }

            existingStudent.Name = updatedStudent.Name;
            existingStudent.Gpa = updatedStudent.Gpa;
            existingStudent.section = updatedStudent.section;

            serviceResponse.Message = "Record updated";
            serviceResponse.Data = existingStudent;


            return serviceResponse;
        }


    }
}