using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTOs
{
    public class ConfirmEmailDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }

        
    }
}