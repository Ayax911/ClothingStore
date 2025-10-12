using System;
using ClothingStore.Domain.Entities;

namespace ClothingStore.Application.Interfaces
{
    public interface IComprasAplicacion
    {
        Task<List<Compras>> ListarAsync();
        Task<List<Compras>> PorCodigoAsync(string codigo);
        Task<Compras?> GuardarAsync(Compras entidad);
        Task<Compras?> ModificarAsync(Compras entidad);
        Task<Compras?> BorrarAsync(Compras entidad);
    }
}
