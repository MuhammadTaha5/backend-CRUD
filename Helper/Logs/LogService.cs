namespace MyFirstAPI.Services
{
    public class LogService : ILogService
    {
        private Guid _id;
        public LogService()
        {
            _id = Guid.NewGuid();
        }
        public Guid GetOperationId()
        {
            return _id;
        }

    }
}