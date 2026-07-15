using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTOs
{
    public class SetPasswordDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Token { get; set; }
        
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
            ErrorMessage = "Password must have 1 uppercase, 1 digit, 1 special character")]
        public string Password { get; set; } = string.Empty;

        //[Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
        

    }
}