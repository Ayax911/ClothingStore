using ClothingStore.Application.DTOs;
using ClothingStore.Application.DTOs.Auth;
using ClothingStore.Application.Interfaces;
using ClothingStore.Domain.Entities;
using ClothingStore.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace ClothingStore.Application.Implementaciones
{
    public class AuthService : IAuthService
    {
        private readonly IConexion _conexion;
        private readonly ITokenService _tokenService;

        public AuthService(IConexion conexion, ITokenService tokenService)
        {
            _conexion = conexion;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Buscar usuario por email
                var usuario = await _conexion.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (usuario == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Credenciales incorrectas"
                    };
                }

                // Verificar si el usuario está activo
                if (!usuario.Activo)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Usuario inactivo"
                    };
                }

                // Verificar contraseña
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.PasswordHash))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Credenciales incorrectas"
                    };
                }

                // Actualizar último acceso
                usuario.UltimoAcceso = DateTime.UtcNow;
                await _conexion.SaveChangesAsync();

                // Generar tokens
                var token = _tokenService.GenerarToken(usuario);
                var refreshToken = _tokenService.GenerarRefreshToken();

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Login exitoso",
                    Token = token,
                    RefreshToken = refreshToken,
                    TokenExpiration = DateTime.UtcNow.AddHours(24),
                    Usuario = new UsuarioDto
                    {
                        Id = usuario.Id,
                        Username = usuario.Username,
                        Email = usuario.Email,
                        Rol = usuario.Rol ?? "Cliente"
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Error al iniciar sesión: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDto> RegistrarAsync(RegistroDto registroDto)
        {
            try
            {
                // Verificar si el email ya existe
                var emailExiste = await _conexion.Usuarios
                    .AnyAsync(u => u.Email == registroDto.Email);

                if (emailExiste)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "El email ya está registrado"
                    };
                }

                // Verificar si el username ya existe
                var usernameExiste = await _conexion.Usuarios
                    .AnyAsync(u => u.Username == registroDto.Username);

                if (usernameExiste)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "El nombre de usuario ya está registrado"
                    };
                }

                // Crear nuevo usuario
                var nuevoUsuario = new Usuarios
                {
                    Username = registroDto.Username,
                    Email = registroDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registroDto.Password),
                    Rol = registroDto.Rol,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                };

                _conexion.Usuarios.Add(nuevoUsuario);
                await _conexion.SaveChangesAsync();

                // Si es cliente, crear registro en tabla Clientes
                if (registroDto.Rol == "Cliente")
                {
                    var cliente = new Clientes
                    {
                        Nombre = registroDto.Username,
                        
                    };

                    _conexion.Clientes.Add(cliente);
                    await _conexion.SaveChangesAsync();

                    // Asociar cliente con usuario
                    nuevoUsuario.ClienteId = cliente.Id;
                    await _conexion.SaveChangesAsync();
                }

                // Generar tokens
                var token = _tokenService.GenerarToken(nuevoUsuario);
                var refreshToken = _tokenService.GenerarRefreshToken();

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Usuario registrado exitosamente",
                    Token = token,
                    RefreshToken = refreshToken,
                    TokenExpiration = DateTime.UtcNow.AddHours(24),
                    Usuario = new UsuarioDto
                    {
                        Id = nuevoUsuario.Id,
                        Username = nuevoUsuario.Username,
                        Email = nuevoUsuario.Email,
                        Rol = nuevoUsuario.Rol ?? "Cliente"
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Error al registrar usuario: {ex.Message}"
                };
            }
        }

        public async Task<bool> CambiarPasswordAsync(int usuarioId, CambiarPasswordDto cambiarPasswordDto)
        {
            try
            {
                var usuario = await _conexion.Usuarios.FindAsync(usuarioId);

                if (usuario == null)
                    return false;

                // Verificar contraseña actual
                if (!BCrypt.Net.BCrypt.Verify(cambiarPasswordDto.PasswordActual, usuario.PasswordHash))
                    return false;

                // Actualizar contraseña
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(cambiarPasswordDto.NuevaPassword);
                await _conexion.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            // Aquí deberías implementar la lógica para validar y refrescar el token
            // Esto requeriría almacenar los refresh tokens en la base de datos
            // Por ahora, retornamos un mensaje indicando que no está implementado
            await Task.CompletedTask; // Para evitar warning de async
            
            return new AuthResponseDto
            {
                Success = false,
                Message = "Funcionalidad de refresh token no implementada aún"
            };
        }

        public async Task<bool> RevocarTokenAsync(int usuarioId)
        {
            try
            {
                var usuario = await _conexion.Usuarios.FindAsync(usuarioId);
                
                if (usuario == null)
                    return false;

                // Aquí podrías agregar lógica adicional para revocar tokens
                // Por ejemplo, actualizar un campo de último token revocado
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}