using System;
using ClothingStore.Domain.Entities;

namespace ClothingStore.Application.Interfaces
{
    public interface IProductosAplicacion
    {
        Task<List<Productos>> ListarAsync();
        Task<List<Productos>> PorNombreAsync(string nombre);
        Task<Productos?> GuardarAsync(Productos entidad);
        Task<Productos?> ModificarAsync(Productos entidad);
        Task<Productos?> BorrarAsync(Productos entidad);
    }
}
