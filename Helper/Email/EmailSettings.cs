namespace StudentManagement.Domain.Models
{
    /// <summary>
    /// takes host, port number, username and passwords from secrets
    /// </summary>
    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public bool UseSsl { get; set; } = true;
    }
}