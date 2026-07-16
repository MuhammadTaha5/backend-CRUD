using System.ComponentModel.DataAnnotations;
using StudentManagement.Attributes;

namespace MyFirstAPI.Models
{

    public class Student
    {
        public int Id { get; set; }
        [Required]
        [Filterable, Sortable]
        public required string Name { set; get; }
        [Filterable, Sortable]
        public int Age { get; set; }
        [Filterable, Sortable]
        public double Gpa { get; set; }
        [Filterable]
        public string section { get; set; } = SectionEnum.A.ToString();
        [Filterable]
        public string Email { get; set; } = string.Empty; public string Department { get; set; } = string.Empty;
    }
}