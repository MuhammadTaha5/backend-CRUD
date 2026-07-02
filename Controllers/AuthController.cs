
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;
using MyFirstAPI.Services;
using StudentManagement.Exceptions;
using StudentManagement.Services;
using StudentManagement.Services.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;
    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _config = configuration;
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
    [HttpPost("registerUser")]
    public async Task<IActionResult> RegisterUser(RegisterDTO dto)
    {
        try
        {
            var registerUser = await _authService.RegisterUser(dto);
            return Ok(registerUser);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
        
    }
    [HttpPost("test-email")]
    [AllowAnonymous]
    public async Task<IActionResult> TestEmail([FromServices] IEmailService emailService, [FromQuery] string email)
    {
        await emailService.SendEmailAsync(
            email,
             "Email from StudentHub(Taha)",
             "<h1>Hello</h1><p>How Are You?</p>"
        );
    
        return Ok("Email Sent");
    }


}