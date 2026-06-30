
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;
using MyFirstAPI.Services;
using StudentManagement.Services.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
            
        }
        try
        {
            var registerService = _authService.Register(dto);
            return Ok(registerService);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
          
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var serviceResponse = await _authService.Login(dto);
            return Ok(serviceResponse);

        }
        catch (UnauthorizedAccessException excep)
        {
            return Unauthorized(new { message = excep.Message });
        }

    }


}