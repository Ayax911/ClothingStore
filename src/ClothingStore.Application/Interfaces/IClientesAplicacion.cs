using System;
using ClothingStore.Domain.Entities;

namespace ClothingStore.Application.Interfaces
{
    
    public interface IClientesAplicacion
    {
        Task<List<Clientes>> PorCedulaAsync(Clientes entidad);
        Task<List<Clientes>> ListarAsync();
        Task<Clientes?> GuardarAsync(Clientes entidad);
        Task<Clientes?> ModificarAsync(Clientes entidad);
        Task<Clientes?> BorrarAsync(Clientes entidad);
    
    }
}

