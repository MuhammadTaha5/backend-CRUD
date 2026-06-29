
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;
using MyFirstAPI.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly TokenService _tokenService;
    private readonly IConfiguration _config;

    public AuthController(UserManager<AppUser> userManager, ApplicationDbContext context,
        TokenService tokenService, IConfiguration config)
    {
        _userManager = userManager;
        _context = context;
        _tokenService = tokenService;
        _config = config;
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

        // Identity hashes password automatically
        var result = await _userManager.CreateAsync(user, dto.Password);
        
        if (!result.Succeeded)  
        {
            // return Identity errors (e.g. password too weak)
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { errors });
        }

        // assign default role
        await _userManager.AddToRoleAsync(user, "Student");

        return Ok(new { message = "Registration successful" });
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials" }); 

        // check lockout
        if (await _userManager.IsLockedOutAsync(user))
            return Unauthorized(new { message = "Account locked. Try again later." });

        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user); 
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // reset fail count on success
        await _userManager.ResetAccessFailedCountAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);

        return Ok(new AuthResponseDTO
        {
            AccessToken = accessToken,
            Email = user.Email!,
            FullName = user.FullName
        });
    }

   
}