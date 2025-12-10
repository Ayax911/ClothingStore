using ClothingStore.Domain.Entities;
using ClothingStore.Application.Interfaces;
using ClothingStore.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClothingStore.Application.Implementaciones
{
    public class DetallesComprasAplicacion : IDetallesComprasAplicacion
    {
        private readonly IConexion _conexion;

        public DetallesComprasAplicacion(IConexion conexion)
        {
            _conexion = conexion;
        }

        public async Task<List<DetallesCompras>> ListarAsync()
        {
            return await _conexion.DetallesCompras!
                .Include(d => d.Compra)
                .Include(d => d.Producto)
                .Take(50)
                .ToListAsync();
        }

        public async Task<List<DetallesCompras>> PorCompraAsync(int compraId)
        {
            return await _conexion.DetallesCompras!
                .Include(d => d.Producto)
                .Where(d => d.CompraId == compraId)
                .ToListAsync();
        }

        public async Task<DetallesCompras?> GuardarAsync(DetallesCompras entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id != 0)
                throw new Exception("lbYaSeGuardo");

            // Validación: asegurar que exista la compra y el producto
            bool compraExiste = await _conexion.Compras!.AnyAsync(c => c.Id == entidad.CompraId);
            bool productoExiste = await _conexion.Productos!.AnyAsync(p => p.Id == entidad.ProductoId);

            if (!compraExiste || !productoExiste)
                throw new Exception("lbFKInvalida");

             if (entidad.Cantidad <= 0 || entidad.Producto == null)
            {
                // Opcional: cargar producto desde DB si no está cargado
                entidad.Producto = await _conexion.Productos!.FindAsync(entidad.ProductoId);
                if (entidad.Producto == null)
                    throw new Exception("Producto no existe");
            }

            entidad.ValorBruto = entidad.Cantidad * entidad.Producto!.ValorUnitario;

            _conexion.DetallesCompras!.Add(entidad);
            await _conexion.SaveChangesAsync();
            return entidad;
        }

        public async Task<DetallesCompras?> ModificarAsync(DetallesCompras entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id == 0)
                throw new Exception("lbNoSeGuardo");

             if (entidad.Cantidad <= 0 || entidad.Producto == null)
            {
                // Opcional: cargar producto desde DB si no está cargado
                entidad.Producto = await _conexion.Productos!.FindAsync(entidad.ProductoId);
                if (entidad.Producto == null)
                    throw new Exception("Producto no existe");
            }

    entidad.ValorBruto = entidad.Cantidad * entidad.Producto!.ValorUnitario;

            var entry = _conexion.Entry(entidad);
            entry.State = EntityState.Modified;
            await _conexion.SaveChangesAsync();
            return entidad;
        }

        public async Task<DetallesCompras?> BorrarAsync(DetallesCompras entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id == 0)
                throw new Exception("lbNoSeGuardo");

            var existente = await _conexion.DetallesCompras.FindAsync(entidad.Id);
            if (existente == null)
                return null;
                

            _conexion.DetallesCompras!.Remove(entidad);
            await _conexion.SaveChangesAsync();
            return entidad;
        }
    }
}
