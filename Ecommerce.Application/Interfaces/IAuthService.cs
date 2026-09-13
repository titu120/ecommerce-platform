using Ecommerce.Application.DTOs.Auth;

namespace Ecommerce.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}