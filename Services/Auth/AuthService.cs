using System.Net;
using Microsoft.AspNetCore.Identity;
using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;
using MyFirstAPI.Services;
using StudentManagement.Constants;
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

        public async Task<ServiceResponse<RegisterDTO>> RegisterUser(RegisterDTO dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new ConflictException("User Already Registered");
            }
            AppUser user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                EmailConfirmed = false
            };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new ValidationException(errors);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var confirmLink = $"{_config["FrontendUrl"]}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";

            var body = $"<p>Hi {user.FullName},</p><p>Click below to set your password and activate your account:</p><a href='{confirmLink}'>Confirm Account</a>";

            await _emailService.SendEmailAsync(user.Email, "Confirm your account", body);

            return new ServiceResponse<RegisterDTO>
            {
                Message = "User Created and Verification. Email Sent.",
                success = true,
                Data = dto
            };
        }
        public async Task<ServiceResponse<Object>> ConfirmEmail(ConfirmEmailDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new NotFoundException("User Not Found");
            }
            if (user.EmailConfirmed)
            {
                throw new ConflictException("Account already verfied");
            }
            var confirmAccount = await _userManager.ConfirmEmailAsync(user, dto.Token);
            if (!confirmAccount.Succeeded)
            {
                var errors = confirmAccount.Errors.Select(e => e.Description).ToList();
                Console.WriteLine("CONFIRM EMAIL ERRORS: " + string.Join(" | ", errors));
                throw new ValidationException(errors);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            Console.WriteLine($"TOKEN LENGTH: {token.Length}");
            Console.WriteLine($"RAW TOKEN: {token}");

            return new ServiceResponse<object>
            {
                Data = new
                {
                    userId = user.Id,
                    token = encodedToken
                },
                success = true,
                Message = "Email Confirmed"
            };

        }
        public async Task<ServiceResponse<AuthResponseDTO>> SetPassword(SetPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user==null)
            {
                throw new NotFoundException("No User Found");
            }
            var setPassword = await _userManager.ResetPasswordAsync(user, dto.Token, dto.Password);
            if (!setPassword.Succeeded)
            {
                var errors = setPassword.Errors.Select(e => e.Description).ToList();
                Console.WriteLine("Password ERRORS: " + string.Join(" | ", errors));
                throw new ValidationException(errors);
            }
            await _userManager.AddToRoleAsync(user, Roles.Student);
            var accessToken = _tokenService.GenerateAccessToken(user, new List<string>{"Student"});

            return new ServiceResponse<AuthResponseDTO>
            {
                Data = new AuthResponseDTO
                {
                    AccessToken = accessToken,
                    Email = user.Email,
                    FullName = user.FullName
                },
                success = true,
                Message = "Logged in Successfully. Token Generated"
            };
        }
        public async Task<ServiceResponse<string>> ForgotPassword(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user==null || !user.EmailConfirmed)
            {
                throw new ValidationException(new List<string>{"Password reset failed"});
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            var resetLink = $"{_config["FrontendUrl"]}/api/auth/set-password?userId={user.Id}&token={encodedToken}";

            var body = $"<p>Hi {user.FullName},</p><p>Click below to reset your password:</p><a href='{resetLink}'>Reset Password</a>";
            Console.WriteLine($"TOKEN LENGTH: {token.Length}");
            Console.WriteLine($"RAW TOKEN: {token}");
            await _emailService.SendEmailAsync(user.Email, "Reset your password", body);
            return new ServiceResponse<string>
            {
                Data = null,
                success = true, 
                Message = "Password reset Email sent"
            };
        }



    }
}