using System.Net;
using AutoMapper;
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
        private readonly Mapper _mapper;
        public AuthService(IConfiguration config, TokenService tokenService, UserManager<AppUser> userManager, IEmailService emailService, Mapper mapper)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
            _emailService = emailService;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<AuthResponseDTO>> Login(LoginDTO loginDTO)
        {
            AppUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedAccessException("Account locked. Try again later.");

            bool passwordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!passwordValid)
            {
                await _userManager.AccessFailedAsync(user);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // reset fail count on success
            await _userManager.ResetAccessFailedCountAsync(user);

            IList<string> roles = await _userManager.GetRolesAsync(user);
            string accessToken = _tokenService.GenerateAccessToken(user, roles);

            return ServiceResponse<AuthResponseDTO>.SuccessResponse(new AuthResponseDTO{
                AccessToken = accessToken,
                Email = user.Email!,
                FullName = user.FullName
            },"Logged in Successfully");
        }

        public async Task<ServiceResponse<RegisterDTO>> RegisterUser(RegisterDTO dto)
        {
            AppUser? existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new ConflictException("User Already Registered");
            }
            AppUser user = _mapper.Map<AppUser>(dto);
            user.EmailConfirmed = false;
            IdentityResult? result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                IEnumerable<string>? errors = result.Errors.Select(e => e.Description);
                throw new ValidationException(errors);
            }
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string encodedToken = WebUtility.UrlEncode(token);
            string confirmLink = $"{_config["FrontendUrl"]}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";

            string body = $"<p>Hi {user.FullName},</p><p>Click below to set your password and activate your account:</p><a href='{confirmLink}'>Confirm Account</a>";

            await _emailService.SendEmailAsync(dto.Email, "Confirm your account", body);

            return  ServiceResponse<RegisterDTO>.SuccessResponse(dto, "User Created. Verification Email Sent.");
            
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