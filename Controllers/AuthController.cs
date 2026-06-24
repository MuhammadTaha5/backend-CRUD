// Controllers/AuthController.cs
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
            return Unauthorized(new { message = "Invalid credentials" }); // don't say "email not found"

        // check lockout
        if (await _userManager.IsLockedOutAsync(user))
            return Unauthorized(new { message = "Account locked. Try again later." });

        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user); // increment fail count
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // reset fail count on success
        await _userManager.ResetAccessFailedCountAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // save refresh token to DB
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(
                double.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]!))
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = user.Email!,
            FullName = user.FullName
        });
    }

    // POST api/auth/refresh
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDTO>> Refresh([FromBody] string refreshToken)
    {
        var tokenEntity = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiresAt < DateTime.UtcNow)
            return Unauthorized(new { message = "Invalid or expired refresh token" });

        // revoke old, issue new (token rotation)
        tokenEntity.IsRevoked = true;

        var roles = await _userManager.GetRolesAsync(tokenEntity.User);
        var newAccessToken = _tokenService.GenerateAccessToken(tokenEntity.User, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = tokenEntity.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        await _context.SaveChangesAsync();

        return Ok(new AuthResponseDTO
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    // POST api/auth/revoke  (logout)
    [HttpPost("revoke")]
    public async Task<ActionResult> Revoke([FromBody] string refreshToken)
    {
        var tokenEntity = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (tokenEntity == null)
            return NotFound();

        tokenEntity.IsRevoked = true;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Logged out successfully" });
    }
}