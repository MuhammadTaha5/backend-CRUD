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
        [MinLength(11)]
        public string PhoneNumber { get; set; } = string.Empty;

    }

}