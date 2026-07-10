namespace MyFirstAPI.Models
{

    public class Student
    {
        public int Id { get; set; }
        public string? Name { set; get; }
        public int Age { get; set; }
        public double Gpa { get; set; }
        public string section { get; set; } = SectionEnum.A.ToString();
        public string Email { get; set; } = string.Empty; public string Department { get; set; } = string.Empty;
    }
}