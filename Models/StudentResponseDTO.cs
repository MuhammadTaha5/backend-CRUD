namespace MyFirstAPI.Models
{
    public class StudentResponseDTO
    {
        public string? Name{set; get;}
        public int Age { get; set; }
        public string Department { get; set; } = string.Empty;
    }
}