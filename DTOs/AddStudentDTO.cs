using System.ComponentModel.DataAnnotations;

public class AddStudentDTO
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 100)]
    public int Age { get; set; }

    [Range(0.0, 4.0)]
    public double Gpa { get; set; }

    [Required]
    public string Section { get; set; } = "A";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Department { get; set; } = string.Empty;
}