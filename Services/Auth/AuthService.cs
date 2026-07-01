using Microsoft.AspNetCore.Identity;
using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;
using MyFirstAPI.Services;
using StudentManagement.Exceptions;

namespace StudentManagement.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;
        public AuthService(IConfiguration config, TokenService tokenService, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
        }
        public async Task<AuthResponseDTO> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedAccessException("Account locked. Try again later.");

            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!passwordValid)
            {
                await _userManager.AccessFailedAsync(user);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // reset fail count on success
            await _userManager.ResetAccessFailedCountAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles);

            return (new AuthResponseDTO
            {
                AccessToken = accessToken,
                Email = user.Email!,
                FullName = user.FullName
            });
        }

        public async Task<ServiceResponse<RegisterDTO>> Register(RegisterDTO dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new ConflictException("Email already registered");
            }
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
                throw new ValidationException(errors);
            }
            await _userManager.AddToRoleAsync(user, "Student");
            return new ServiceResponse<RegisterDTO>{
                success = true,
                Message = "Registration successful",
                Data = dto
            };


        }

    }
}