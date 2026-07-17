namespace StudentManagement.Domain.Models
{
    /// <summary>
    /// takes host, port number, username and passwords from secrets
    /// </summary>
    public class EmailSettings
    {
        public required string Host { get; set; }
        public required int Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FromEmail { get; set; }
        public required string FromName { get; set; }
        public bool UseSsl { get; set; } = true;
    }
}