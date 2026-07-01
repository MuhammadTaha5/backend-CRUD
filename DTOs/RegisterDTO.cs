using System.ComponentModel.DataAnnotations;
namespace MyFirstAPI.Models.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [MinLength(3)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
            ErrorMessage = "Password must have 1 uppercase, 1 digit, 1 special character")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

}