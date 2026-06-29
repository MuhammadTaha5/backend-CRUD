namespace MyFirstAPI.Models
{
    public class StudentResponseDTO
    {
        public int Id { get; set; }
        public string? Name{set; get;}
        public int Age { get; set; }
        public string Department { get; set; } = string.Empty;
    }
}