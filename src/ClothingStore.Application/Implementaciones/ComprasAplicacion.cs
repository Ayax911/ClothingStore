using ClothingStore.Domain.Entities;
using ClothingStore.Application.Interfaces;
using ClothingStore.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClothingStore.Application.Implementaciones
{
    public class ComprasAplicacion : IComprasAplicacion
    {
        private readonly IConexion _conexion;

        public ComprasAplicacion(IConexion conexion)
        {
            _conexion = conexion;
        }

        public async Task<List<Compras>> ListarAsync()
        {
            return await _conexion.Compras!
                .Include(c => c.Cliente)
                .Include(c => c.DetallesCompras!)
                    .ThenInclude(d => d.Producto)
                .OrderByDescending(c => c.Fecha)
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<Compras>> PorCodigoAsync(string codigo)
        {
            return await _conexion.Compras!
                .Include(c => c.Cliente)
                .Include(c => c.DetallesCompras!)
                    .ThenInclude(d => d.Producto)
                .Where(c => c.Codigo!.Contains(codigo))
                .ToListAsync();
        }

        public async Task<Compras?> GuardarAsync(Compras entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id != 0)
                throw new Exception("lbYaSeGuardo");

            // Asegura que no se repita el código
            bool existe = await _conexion.Compras!.AnyAsync(c => c.Codigo == entidad.Codigo);
            if (existe)
                throw new Exception("lbCodigoExistente");

            //Calcular ValorBruto de cada detalle si no está calculado
            foreach (var detalle in entidad.DetallesCompras!)
            {
                var producto = await _conexion.Productos!.FindAsync(detalle.ProductoId);
                if (producto == null)
                    throw new Exception($"Producto con Id {detalle.ProductoId} no existe");

                detalle.Producto = producto;
                detalle.ValorBruto = detalle.Cantidad * producto.ValorUnitario;
            }

            // Calcular ValorTotal de la compra
            entidad.ValorTotal = entidad.DetallesCompras.Sum(d => d.ValorBruto);


            _conexion.Compras!.Add(entidad);
            await _conexion.SaveChangesAsync();
            return entidad;
        }

        public async Task<Compras?> ModificarAsync(Compras entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id == 0)
                throw new Exception("lbNoSeGuardo");

            var entry = _conexion.Entry(entidad);
            entry.State = EntityState.Modified;
            await _conexion.SaveChangesAsync();
            return entidad;
        }

        public async Task<Compras?> BorrarAsync(Compras entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id == 0)
                throw new Exception("lbNoSeGuardo");

            var existente = await _conexion.Compras.FindAsync(entidad.Id);
            if (existente == null)
                return null;
                

            _conexion.Compras!.Remove(entidad);
            await _conexion.SaveChangesAsync();
            return entidad;
        }
    }
}
