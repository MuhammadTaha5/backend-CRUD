using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        public required string Email { get; set; }
    }
}