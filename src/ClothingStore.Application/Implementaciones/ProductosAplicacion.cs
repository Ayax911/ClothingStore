using ClothingStore.Domain.Entities;
using ClothingStore.Application.Interfaces;
using ClothingStore.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClothingStore.Application.Implementaciones
{
    public class ProductosAplicacion : IProductosAplicacion
    {
        private readonly IConexion _conexion;

        public ProductosAplicacion(IConexion conexion)
        {
            _conexion = conexion;
        }

        public async Task<List<Productos>> ListarAsync()
        {
            return await _conexion.Productos!
                .OrderBy(p => p.Nombre)
                .Take(50)
                .ToListAsync();
        }

        public async Task<List<Productos>> PorNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("lbFaltaInformacion");

            return await _conexion.Productos!
                .Where(p => p.Nombre!.Contains(nombre))
                .ToListAsync();
        }

        public async Task<Productos?> GuardarAsync(Productos entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id != 0)
                throw new Exception("lbYaSeGuardo");

            bool existeCodigo = await _conexion.Productos!
                .AnyAsync(p => p.Codigo == entidad.Codigo);

            if (existeCodigo)
                throw new Exception("lbCodigoExistente");

            _conexion.Productos!.Add(entidad);
            await _conexion.SaveChangesAsync();
            return entidad;
        }

        public async Task<Productos?> ModificarAsync(Productos entidad)
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

        public async Task<Productos?> BorrarAsync(Productos entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id == 0)
                throw new Exception("lbNoSeGuardo");

            _conexion.Productos!.Remove(entidad);
            await _conexion.SaveChangesAsync();
            return entidad;
        }
    }
}
