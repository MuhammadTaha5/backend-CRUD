using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;
using StudentManagement.Controllers;
using StudentManagement.DTOs;
using StudentManagement.Exceptions;
using StudentManagement.Services;
using StudentManagement.Services.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;
    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _config = configuration;
    }
    /// <summary>
    /// Authenticates a user with email and password and returns a JWT access token on success.
    /// </summary>
    /// <param name="dto">The login credentials.</param>
    /// <returns>An access token and user info on success, or 401 if authentication fails.</returns>
    /// <response code="200">Login successful; access token returned.</response>
    /// <response code="400">Request body failed validation.</response>
    /// <response code="401">Invalid credentials, account locked, or other authentica

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        try
        {
            AuthResponseDTO serviceResponse = await _authService.Login(dto);
            return OkResponse(serviceResponse, "Logged-in Successfully");

        }
        catch (UnauthorizedAccessException excep)
        {
            return Unauthorized(ServiceResponse<string>.FailResponse(excep.Message, null));
        }


    }
    /// <summary>
    /// registers the user by username, email, contact number. and send email verifcation token.
    /// </summary>
    /// <param name="dto">The registration credentials</param>
    /// <returns>gives the if all registeration checks pass email verfication token.</returns>
    [HttpPost("registerUser")]
    public async Task<IActionResult> RegisterUser(RegisterDTO dto)
    {
        try
        {
            RegisterDTO registerUser = await _authService.RegisterUser(dto);
            return OkResponse(registerUser, "Registered Successfully, Email Sent");
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
    /// <summary>
    /// Confirms the user acccount by verfication of userId, and token.
    /// </summary>
    /// <param name="userId">the ID of user in database.</param>
    /// <param name="token">the email verfication token</param>
    /// <returns>The confirmation message and Set password token if successfull. Else give the bad request</returns>
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        try
        {
            ConfirmEmailDto confirmEmail = new ConfirmEmailDto{UserId = userId,Token = token};
            object userConfirm = await _authService.ConfirmEmail(confirmEmail);
            return OkResponse(userConfirm, "Email Confirmed. Use Token to set Password");
        }
        catch (Exception e)
        {
            //email already confirmed or user does not exist
            return BadRequest(ServiceResponse<string>.FailResponse(e.Message, null));
        }

    }
    /// <summary>
    /// Sets the password after token verication and give access token.
    /// </summary>
    /// <param name="setPasswordDto">New password and confirm password</param>
    /// <returns>the success message of password set and generate the access token. if not then give bad request response</returns>
    [HttpPost("set-password")]
    [AllowAnonymous]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto setPasswordDto)
    {
        try
        {
            AuthResponseDTO setPassword = await _authService.SetPassword(setPasswordDto);
            return OkResponse(setPassword, "Token Generated, Logged-in Successfully");
        }
        catch (ValidationException e)
        {
            //if token not validated or inserting new password issue
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            //if no user exist in database
            return BadRequest(ServiceResponse<string>.FailResponse(e.Message, null));
        }



    }
    /// <summary>
    /// sends the password reset token on email. and validate the token on request
    /// </summary>
    /// <param name="dto">the email address of user</param>
    /// <returns>the success response of email sent.</returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        try
        {
            string response = await _authService.ForgotPassword(dto);
            return OkResponse(response, "Reset Password Email Sent");
            
        }
        catch (Exception e)
        {
            //catch exception if the user is not confirmed or issue in email sending.
            return BadRequest(ServiceResponse<string>.FailResponse(e.Message, null));
            
        }
  
    }
    /// <summary>
    /// Tests the email service  working properly.
    /// </summary>
    /// <param name="emailService"></param>
    /// <param name="email"></param>
    /// <returns></returns>

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