using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;
using MyFirstAPI.Services;
using StudentManagement.DTOs;
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

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<ServiceResponse<AuthResponseDTO>>> Login(LoginDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            ServiceResponse<AuthResponseDTO> serviceResponse = await _authService.Login(dto);
            return Ok(serviceResponse);

        }
        catch (UnauthorizedAccessException excep)
        {
            return Unauthorized(ServiceResponse<string>.FailResponse(excep.Message, null));
        }


    }
    [HttpPost("registerUser")]
    public async Task<IActionResult> RegisterUser(RegisterDTO dto)
    {
        try
        {
            ServiceResponse<RegisterDTO> registerUser = await _authService.RegisterUser(dto);
            return Ok(registerUser);
        }
        catch (ConflictException e)
        {
            return Conflict(ServiceResponse<string>.FailResponse(e.Message, null));
        }
        catch (Exception e)
        {
            return BadRequest(ServiceResponse<string>.FailResponse(e.Message, null));
        }
    }

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<ActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            ConfirmEmailDto confirmEmail = new ConfirmEmailDto
            {
                UserId = userId,
                Token = token
            };
            ServiceResponse<object> userConfirm = await _authService.ConfirmEmail(confirmEmail);
            return Ok(userConfirm);
        }
        catch (Exception e)
        {
            return BadRequest(ServiceResponse<string>.FailResponse(e.Message, null));
        }

    }
    [HttpPost("set-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<AuthService>>> SetPassword([FromBody] SetPasswordDto setPasswordDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            ServiceResponse<AuthResponseDTO> setPassword = await _authService.SetPassword(setPasswordDto);
            return Ok(setPassword);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return BadRequest(ServiceResponse<string>.FailResponse(e.Message, null));
        }



    }
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        try
        {
            ServiceResponse<string> response = await _authService.ForgotPassword(dto);
            return Ok(response);
            
        }
        catch (Exception e)
        {
            return BadRequest(ServiceResponse<string>.FailResponse(e.Message, null));
            
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