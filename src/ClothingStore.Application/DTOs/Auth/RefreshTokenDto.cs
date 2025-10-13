
using System.ComponentModel.DataAnnotations;

namespace ClothingStore.Application.DTOs.Auth
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "El refresh token es requerido")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}

