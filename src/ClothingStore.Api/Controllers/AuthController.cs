using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClothingStore.Application.Interfaces;
using ClothingStore.Application.DTOs;
using ClothingStore.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace ClothingStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Iniciar sesión
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _authService.LoginAsync(loginDto);

            if (!resultado.Success)
                return Unauthorized(new { message = resultado.Message });

            return Ok(resultado);
        }

        /// <summary>
        /// Registrar nuevo usuario
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistroDto registroDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _authService.RegistrarAsync(registroDto);

            if (!resultado.Success)
                return BadRequest(new { message = resultado.Message });

            return Ok(resultado);
        }

        /// <summary>
        /// Cambiar contraseña del usuario autenticado
        /// </summary>
        [HttpPost("cambiar-password")]
        [Authorize]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDto cambiarPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Obtener el ID del usuario del token
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (usuarioId == 0)
                return Unauthorized(new { message = "Usuario no autenticado" });

            var resultado = await _authService.CambiarPasswordAsync(usuarioId, cambiarPasswordDto);

            if (!resultado)
                return BadRequest(new { message = "No se pudo cambiar la contraseña. Verifica tu contraseña actual." });

            return Ok(new { message = "Contraseña cambiada exitosamente" });
        }

        /// <summary>
        /// Refrescar token de acceso
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var resultado = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);

            if (!resultado.Success)
                return Unauthorized(new { message = resultado.Message });

            return Ok(resultado);
        }

        /// <summary>
        /// Revocar token del usuario autenticado (logout)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (usuarioId == 0)
                return Unauthorized(new { message = "Usuario no autenticado" });

            var resultado = await _authService.RevocarTokenAsync(usuarioId);

            if (!resultado)
                return BadRequest(new { message = "No se pudo cerrar la sesión" });

            return Ok(new { message = "Sesión cerrada exitosamente" });
        }

        /// <summary>
        /// Obtener información del usuario autenticado
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                id = usuarioId,
                username = username,
                email = email,
                rol = rol
            });
        }
    }
}
