using ClothingStore.Application.DTOs.Auth;
using ClothingStore.Application.DTOs; 

namespace ClothingStore.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegistrarAsync(RegistroDto registroDto);
        Task<bool> CambiarPasswordAsync(int usuarioId, CambiarPasswordDto cambiarPasswordDto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> RevocarTokenAsync(int usuarioId);
    }
}