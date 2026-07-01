using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;

namespace StudentManagement.Services.Auth
{
    public interface IAuthService
    {
        public Task<ServiceResponse<RegisterDTO>> Register(RegisterDTO dto);
        public Task<AuthResponseDTO> Login(LoginDTO loginDTO);
    }
}