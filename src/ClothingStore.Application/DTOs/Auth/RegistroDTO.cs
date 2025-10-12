using System;
using System.ComponentModel.DataAnnotations;

namespace ClothingStore.Application.DTOs.Auth
{
    public class RegistroDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede tener más de 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [MaxLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [MaxLength(100, ErrorMessage = "La contraseña no puede tener más de 100 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio")]
        [RegularExpression("^(Cliente|Vendedor)$", ErrorMessage = "El rol debe ser Cliente o Vendedor")]
        public string Rol { get; set; } = "Cliente";
    }
}

