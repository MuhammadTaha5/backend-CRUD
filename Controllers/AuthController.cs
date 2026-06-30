
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
            return BadRequest(ModelState);

        // check duplicate email
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return Conflict(new { message = "Email already registered" });

        var user = new AppUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email  // Identity requires UserName
        };

        
        var result = await _userManager.CreateAsync(user, dto.Password);
        
        if (!result.Succeeded)  
        {
        
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { errors });
        }

        
        await _userManager.AddToRoleAsync(user, "Student");

        return Ok(new { message = "Registration successful" });
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
            return Unauthorized(new {message= excep.Message});
        }
        


    }

   
}