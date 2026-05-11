namespace MyFirstAPI.Models
{
    public class Students
    {
        public int Id
        {
            get;
            set;
        }
        public string? Name
        {
            set;
            get;
        }
        public double Gpa
        {
            get;
            set;
        }
        public string section { get; set; } = SectionEnum.A.ToString();
    }
}