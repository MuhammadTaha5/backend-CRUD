using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;
using StudentManagement.DTOs;

namespace StudentManagement.Services.Auth
{
    public interface IAuthService
    {
        public Task<ServiceResponse<RegisterDTO>> Register(RegisterDTO dto);
        public Task<AuthResponseDTO> Login(LoginDTO loginDTO);
        public Task<ServiceResponse<RegisterDTO>> RegisterUser(RegisterDTO dto);
        public Task<ServiceResponse<String>> ConfirmEmail(ConfirmEmailDto dto);
    }
}