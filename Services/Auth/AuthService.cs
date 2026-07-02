using System.Net;
using Microsoft.AspNetCore.Identity;
using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;
using MyFirstAPI.Services;
using StudentManagement.DTOs;
using StudentManagement.Exceptions;

namespace StudentManagement.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        public AuthService(IConfiguration config, TokenService tokenService, UserManager<AppUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
            _emailService = emailService;
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
        
        public async Task<ServiceResponse<RegisterDTO>> RegisterUser(RegisterDTO dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if(existingUser!=null)
            {
                throw new ConflictException("User Already Registered");
            }
            AppUser user = new AppUser
            {
                UserName=dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                EmailConfirmed = false
            };
            var result = await _userManager.CreateAsync(user);
            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new ValidationException(errors);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var confirmLink = $"{_config["FrontendUrl"]}/confirm-account?userId={user.Id}&token={encodedToken}";
            
            var body = $"<p>Hi {user.FullName},</p><p>Click below to set your password and activate your account:</p><a href='{confirmLink}'>Confirm Account</a>";

            await _emailService.SendEmailAsync(user.Email, "Confirm your account", body);
            return new ServiceResponse<RegisterDTO>
            {
                Message = "User Created and Verification. Email Sent.",
                success = true,
                Data = dto
            };
        }
        public async Task<ServiceResponse<String>> ConfirmEmail(ConfirmEmailDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if(user==null)
            {
                throw new NotFoundException("User Not Found");
            }
            if(user.EmailConfirmed)
            {
                throw new ConflictException("Account already verfied");
            }
            var confirmAccount = await _userManager.ConfirmEmailAsync(user, dto.Token);
            if (!confirmAccount.Succeeded)
            {
                var errors = confirmAccount.Errors.Select(e => e.Description);
                throw new ValidationException(errors);
            }
            var confirmPassword = await _userManager.AddPasswordAsync(user, dto.NewPassword);
            if(!confirmPassword.Succeeded)
            {
                var errors = confirmPassword.Errors.Select(e => e.Description);
                throw new ValidationException(errors);   
            }
            return new ServiceResponse<string>
            {
                Data=user.Id,
                success = true,
                Message = "Account Confirmed"
                
            };



        }
        


    }
}