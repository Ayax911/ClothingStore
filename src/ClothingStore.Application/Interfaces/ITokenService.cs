using System;
using ClothingStore.Domain.Entities;

namespace ClothingStore.Application.Interfaces
{
    
    public interface ITokenService
    {
        string GenerarToken(Usuarios usuario);
        string GenerarRefreshToken();
        bool ValidarToken(string token);
    }
}


