using System.ComponentModel.DataAnnotations;
namespace MyFirstAPI.Models
{
    public class UpdateStudentDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 100)]
        public int Age { get; set; }

        [Required]
        public string Section { get; set; } = "A";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

    }
}