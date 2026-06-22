using AutoMapper;
using MyFirstAPI.Models;
namespace MyFirstAPI.Services
{
    public class StudentService : IStudentService
    {
        public List<Student> students;
        private readonly IMapper _mapper;
        public StudentService(IMapper mapper)
        {
            _mapper = mapper;
            students = new List<Student>
            {
                new Student
                {
                    Id = 132,
                    Name = "Muhammad Taha",
                    Gpa = 3.79,
                    Age = 21,
                    Department = "Computer Science",
                    Email = "taha.saeed339@gmail.com"

                },
                new Student
                {
                    Id = 108,
                    Name = "Muhammad Faisal",
                    Gpa = 3.08,
                    Age = 21,
                    Department = "Computer Science",
                    Email = "faisal@gmail.com"

                },
                new Student
                {
                    Id = 19,
                    Name = "Abdullah",
                    Gpa = 3.44,
                    Age = 21,
                    Department = "Computer Science",
                    Email = "abdullah@gmail.com"

                }

            };
        }

        public ServiceResponse<StudentResponseDTO> GetStudentById(int id)
        {
            ServiceResponse<StudentResponseDTO> serviceResponse = new ServiceResponse<StudentResponseDTO>();

            var record = students.FirstOrDefault(s => s.Id == id);

            if (record != null)
            {
                var studentResponseDTO = _mapper.Map<StudentResponseDTO>(record);
                serviceResponse.Data = studentResponseDTO;
                serviceResponse.Message = "Record Found";
                return serviceResponse;
            }
            serviceResponse.Data = null;
            serviceResponse.Message = $"No record Found with ID {id}";
            serviceResponse.success = false;
            return serviceResponse;
        }

        public ServiceResponse<List<StudentResponseDTO>> GetAllStudents()
        {
            ServiceResponse<List<StudentResponseDTO>> serviceResponse = new ServiceResponse<List<StudentResponseDTO>>();
            var studentResponseDTOs = _mapper.Map<List<StudentResponseDTO>>(students);

            serviceResponse.Data = studentResponseDTOs;
            serviceResponse.Message = "Students Retrieved Successfully";
            serviceResponse.success = true;

            return serviceResponse;
        }
        public ServiceResponse<Student> AddStudent(Student std)
        {
            ServiceResponse<Student> serviceResponse = new ServiceResponse<Student>();

            students.Add(std);
            serviceResponse.Data = std;
            serviceResponse.Message = "New Record Added";
            return serviceResponse;
        }
        public ServiceResponse<Student> RemoveStudent(int id)
        {
            ServiceResponse<Student> serviceResponse = new ServiceResponse<Student>();

            var getStudentRecord = students.FirstOrDefault(c => (c.Id == id));
            if (getStudentRecord == null)
            {
                serviceResponse.Message = $"No student record found with Id {id}";
                serviceResponse.success = false;

            }
            if (getStudentRecord != null)
            {
                students.Remove(getStudentRecord);
            }

            serviceResponse.Data = getStudentRecord;
            serviceResponse.Message = "Record removed";

            return serviceResponse;
        }
        public ServiceResponse<Student> UpdateStudent(int id, Student updatedStudent)
        {
            // Find existing student
            ServiceResponse<Student> serviceResponse = new ServiceResponse<Student>();

            var existingStudent = students.FirstOrDefault(c => (c.Id == id));

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

        public ServiceResponse<List<StudentResponseDTO>> GetStudentByName(string name)
        {
            ServiceResponse<List<StudentResponseDTO>> serviceResponse = new ServiceResponse<List<StudentResponseDTO>>();
            #pragma warning disable CS8602 // Dereference of a possibly null reference.

            var getRecord = students.Where(s => s.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (getRecord != null)
            {
                var serviceResponseDTO = _mapper.Map<List<StudentResponseDTO>>(getRecord);
                serviceResponse.Data = serviceResponseDTO;
                serviceResponse.Message = "Record Found";
                serviceResponse.success = true;
                return serviceResponse;
            }
            serviceResponse.Data = null;
            serviceResponse.success = false;
            serviceResponse.Message = "No record found";
            return serviceResponse;






        }
    }
}