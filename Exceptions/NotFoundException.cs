namespace StudentManagement.Exceptions
{
    public class NotFoundException:Exception
    {
        /// <summary>
        /// If required resource not exist
        /// </summary>
        /// <param name="message">Reason for resource not existing</param>
        public NotFoundException(string message) : base(message) { }
    }
}