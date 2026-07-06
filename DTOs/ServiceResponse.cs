namespace MyFirstAPI.Models
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public static ServiceResponse<T> SuccessResponse(T data, string message = "Operation successful")
        {
            return new ServiceResponse<T>
            {
                Data = data,
                success = true,
                Message = message
            };
        }

        public static ServiceResponse<T> FailResponse(string message, T? data = default)
        {
            return new ServiceResponse<T>
            {
                Data = data,
                success = false,
                Message = message
            };
        }

        public static ServiceResponse<T> NotFoundResponse(string message)
        {
            return new ServiceResponse<T>
            {
                Data = default,
                success = false,
                Message = message
            };
        }

    }
}