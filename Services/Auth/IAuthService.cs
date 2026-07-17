using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;
using StudentManagement.DTOs;

namespace StudentManagement.Services.Auth
{
    public interface IAuthService
    {
        public Task<AuthResponseDTO> Login(LoginDTO loginDTO);
        public Task<RegisterDTO> RegisterUser(RegisterDTO dto);
        public Task<object> ConfirmEmail(ConfirmEmailDto dto);
        public Task<AuthResponseDTO> SetPassword(SetPasswordDto dto);
        public Task<string> ForgotPassword(ForgotPasswordDto dto);
    }
}