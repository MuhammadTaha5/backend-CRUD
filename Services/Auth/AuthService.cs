using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;
using MyFirstAPI.Services;
using StudentManagement.Constants;
using StudentManagement.DTOs;
using StudentManagement.Exceptions;
using StudentManagement.Helper.Email;

namespace StudentManagement.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        public AuthService(IConfiguration config, TokenService tokenService, 
                            UserManager<AppUser> userManager, IEmailService emailService, 
                            IMapper mapper)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
            _emailService = emailService;
            _mapper = mapper;
        }
        /// <summary>
        /// Authenticates a user by email and password and returning a jwt access token on success
        /// </summary>
        /// <param name="loginDTO">The Login Credentials(email and Password)</param>
        /// <returns> A <see cref="ServiceResponse{AuthResponseDTO}" containing the access token and user info </returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown exception of unauthorized if the email or password not correct and also 
        /// if the account is locked
        /// </exception>
        /// 
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
            //get roles of user from db
            IList<string> roles = await _userManager.GetRolesAsync(user);
            //generate access token
            string accessToken = _tokenService.GenerateAccessToken(user, roles);

            return ServiceResponse<AuthResponseDTO>.SuccessResponse(new AuthResponseDTO{
                AccessToken = accessToken,
                Email = user.Email!,
                FullName = user.FullName
            },"Logged in Successfully");
        }
        /// <summary>
        /// Register A user without password and send verification Link through email with
        /// it user set the password
        /// </summary>
        /// <param name="dto">The Registration Details Email, Username, and contact number</param>
        /// <returns><see cref="ServiceResponse{RegisterDTO}"/> confirming the account is created and verifcation enail is sent</returns>
        /// <exception cref="ConflictException">Throws if the email already exist</exception>
        /// <exception cref="ValidationException">thrown if db fails to insert user in db</exception>
        [Authorize(Roles =Roles.Admin)]
        public async Task<ServiceResponse<RegisterDTO>> RegisterUser(RegisterDTO dto)
        {
            AppUser? existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new ConflictException("User Already Registered");
            }
            AppUser user = _mapper.Map<AppUser>(dto);
            //Setting the username to email
            user.UserName = dto.Email;    

            user.EmailConfirmed = false;
            IdentityResult? result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                IEnumerable<string>? errors = result.Errors.Select(e => e.Description);
                Console.WriteLine($"Errors: {string.Join(", ", errors)}");
                throw new ValidationException(errors);
            }
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string encodedToken = WebUtility.UrlEncode(token);
            string confirmLink = $"{_config["FrontendUrl"]}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";

            string body = EmailTemplates.ActivateAccount(user.FullName, confirmLink);

            await _emailService.SendEmailAsync(dto.Email, "Confirm your account", body);

            return  ServiceResponse<RegisterDTO>.SuccessResponse(dto, "User Created. Verification Email Sent.");
            
        }
        /// <summary>
        /// Confirm if the email is active by sending back the token and user id and 
        /// validating in db
        /// 
        /// </summary>
        /// <param name="dto">User ID, and email verification token</param>
        /// <returns><see cref="ServiceResponse{Object}"/> confirming email is verified and set 
        /// password token is generated.
        /// </returns>
        /// <exception cref="NotFoundException">thrown if no user exist with param user id</exception>
        /// <exception cref="ConflictException">thrown if the user is already verified.</exception>
        /// <exception cref="ValidationException">thrown it the token is not validated.</exception>
        public async Task<ServiceResponse<Object>> ConfirmEmail(ConfirmEmailDto dto)
        {
            AppUser? user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new NotFoundException("User Not Found");
            }
            //if already email confirmed throw exception
            if (user.EmailConfirmed)
            {
                throw new ConflictException("Account already verfied");
            }
            IdentityResult confirmAccount = await _userManager.ConfirmEmailAsync(user, dto.Token);
            if (!confirmAccount.Succeeded)
            {
                List<string> errors = confirmAccount.Errors.Select(e => e.Description).ToList();
                //Console the errors
                Console.WriteLine("CONFIRM EMAIL ERRORS: " + string.Join(" | ", errors));
                throw new ValidationException(errors);
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //token to console temporary for using in resquest without decoding
            string encodedToken = WebUtility.UrlEncode(token);
            Console.WriteLine($"TOKEN LENGTH: {token.Length}");
            //displat raw token
            Console.WriteLine($"RAW TOKEN: {token}");
            //Send generic response for confirmation
            return ServiceResponse<object>.SuccessResponse(new
                {
                    userId = user.Id,
                    token = encodedToken
                }, "Email Confirmed");
            

        }
        /// <summary>
        /// Check the token in param, validate and set new Password
        /// </summary>
        /// <param name="dto">Takes userId, resetPasword token, New Password, Conform password</param>
        /// <returns> <see cref="ServiceResponse{AuthResponseDTO}"/> 
        /// Returns jwt access token in response with username and email.
        /// </returns>
        /// <exception cref="NotFoundException">thrown if no user exist with this Id.</exception>
        /// <exception cref="ValidationException">thrown if the password reset token is not validated.</exception>
        public async Task<ServiceResponse<AuthResponseDTO>> SetPassword(SetPasswordDto dto)
        {
            AppUser? user = await _userManager.FindByIdAsync(dto.UserId);
            if (user==null)
            {
                throw new NotFoundException("No User Found");
            }
            IdentityResult setPassword = await _userManager.ResetPasswordAsync(user, dto.Token, dto.Password);
            //if token not validated or issue in setting new password.
            if (!setPassword.Succeeded)
            {
                List<string> errors = setPassword.Errors.Select(e => e.Description).ToList();
                
                throw new ValidationException(errors);
            }
            await _userManager.AddToRoleAsync(user, Roles.Student);
            string accessToken = _tokenService.GenerateAccessToken(user, new List<string>{"Student"});

            return ServiceResponse<AuthResponseDTO>.SuccessResponse(new AuthResponseDTO
                {
                    AccessToken = accessToken,
                    Email = user.Email,
                    FullName = user.FullName
                }, "Logged in Successfully. Token Generated");
            
        }
        /// <summary>
        /// Email check and send the password reset email with verification token. and if anyother 
        /// error occur giving the same message
        /// </summary>
        /// <param name="dto">
        /// User Id, token and new Password.
        /// </param>
        /// <returns> <see cref="ServiceResponse{string}" /> confirming the email was sent.</returns>
        /// <exception cref="ValidationException">Thown if the token validation or any other setting password error occurs</exception>
        public async Task<ServiceResponse<string>> ForgotPassword(ForgotPasswordDto dto)
        {
            AppUser? user = await _userManager.FindByEmailAsync(dto.Email);
            if (user==null || !user.EmailConfirmed)
            {
                throw new ValidationException(new List<string>{"Password reset failed"});
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string encodedToken = WebUtility.UrlEncode(token);

            string resetLink = $"{_config["FrontendUrl"]}/api/auth/set-password?userId={user.Id}&token={encodedToken}";

            string body = EmailTemplates.PasswordReset(user.FullName, resetLink);
            Console.WriteLine($"TOKEN LENGTH: {token.Length}");
            Console.WriteLine($"RAW TOKEN: {token}");
            await _emailService.SendEmailAsync(user.Email, "Reset your password", body);
            return ServiceResponse<string>.SuccessResponse(null, "Password reset Email sent");
            
        }

    }
}