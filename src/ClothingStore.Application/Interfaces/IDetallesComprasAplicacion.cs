using System;
using ClothingStore.Domain.Entities;

namespace ClothingStore.Application.Interfaces
{
    public interface IDetallesComprasAplicacion
    {
        Task<List<DetallesCompras>> ListarAsync();
        Task<List<DetallesCompras>> PorCompraAsync(int compraId);
        Task<DetallesCompras?> GuardarAsync(DetallesCompras entidad);
        Task<DetallesCompras?> ModificarAsync(DetallesCompras entidad);
        Task<DetallesCompras?> BorrarAsync(DetallesCompras entidad);
    }
}
