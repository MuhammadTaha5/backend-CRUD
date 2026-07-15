using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;
using StudentManagement.DTOs;

namespace StudentManagement.Services.Auth
{
    public interface IAuthService
    {
        public Task<ServiceResponse<AuthResponseDTO>> Login(LoginDTO loginDTO);
        public Task<ServiceResponse<RegisterDTO>> RegisterUser(RegisterDTO dto);
        public Task<ServiceResponse<Object>> ConfirmEmail(ConfirmEmailDto dto);
        public Task<ServiceResponse<AuthResponseDTO>> SetPassword(SetPasswordDto dto);
        public Task<ServiceResponse<string>> ForgotPassword(ForgotPasswordDto dto);
    }
}