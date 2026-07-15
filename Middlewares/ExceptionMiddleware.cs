using System.Text.Json;
using MyFirstAPI.Models;

namespace StudentManagement.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(JsonSerializer.Serialize(ServiceResponse<string>.FailResponse($"Something went wrong. {ex.Message}", null)));
            }
        }
    }
}