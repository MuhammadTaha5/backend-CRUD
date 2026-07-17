using Microsoft.AspNetCore.Mvc;
using MyFirstAPI.Models;

namespace StudentManagement.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>200 OK — success always true, data required, message optional.</summary>
        protected IActionResult OkResponse<T>(T data, string message)
        {
            ServiceResponse<T> response = ServiceResponse<T>.SuccessResponse(data, message);
            return Ok(response);
        }
        /// <summary>400 Bad Request — no data, message required.</summary>
        protected IActionResult BadRequestResponse(string message)
        {
            ServiceResponse<object> response = ServiceResponse<Object>.FailResponse(message);
            return BadRequest(response);
        }
        /// <summary>404 Not Found — no data, message required.</summary>
        protected IActionResult NotFoundResponse(string message)
        {
            ServiceResponse<object> response = ServiceResponse<object>.NotFoundResponse(message);
            return NotFound(response);
        }
        /// <summary>401 Unauthorized — no data, message required.</summary>
        protected IActionResult UnauthorizedResponse(string message)
        {
            ServiceResponse<object> response = ServiceResponse<object>.FailResponse(message);
            return Unauthorized(response);
        }


    }
}