namespace StudentManagement.Exceptions
{
    public class ConflictException : Exception
{
    /// <summary>
    /// This exception if conflict occurs with the original data
    /// </summary>
    /// <param name="message"></param>
    public ConflictException(string message) : base(message) { }
}
}