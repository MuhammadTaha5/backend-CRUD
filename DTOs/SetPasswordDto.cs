using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTOs
{
    public class SetPasswordDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Token { get; set; }

    }
}