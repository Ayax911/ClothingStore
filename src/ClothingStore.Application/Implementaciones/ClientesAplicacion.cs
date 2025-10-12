using System;
using ClothingStore.Application.Interfaces;
using ClothingStore.Domain.Entities;
using ClothingStore.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace ClothingStore.Application.Implementaciones
{
    public class ClientesAplicacion : IClientesAplicacion
    {
      private readonly IConexion _conexion;

        public ClientesAplicacion(IConexion conexion)
        {
            _conexion = conexion;
        }

        public async Task<Clientes?> GuardarAsync(Clientes entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id != 0)
                throw new Exception("lbYaSeGuardo");

            _conexion.Clientes!.Add(entidad);
            await _conexion.SaveChangesAsync();
            return entidad;
        }

        public async Task<Clientes?> ModificarAsync(Clientes entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id == 0)
                throw new Exception("lbNoSeGuardo");

            // ejemplo de modificación
            entidad.Nombre = "Prueba Interfaces";

            var entry = _conexion.Entry(entidad);
            entry.State = EntityState.Modified;
            await _conexion.SaveChangesAsync();
            return entidad;
        }

        public async Task<Clientes?> BorrarAsync(Clientes entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id == 0)
                throw new Exception("lbNoSeGuardo");

            _conexion.Clientes!.Remove(entidad);
            await _conexion.SaveChangesAsync();
            return entidad;
        }

        public async Task<List<Clientes>> ListarAsync()
        {
            return await _conexion.Clientes!
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<Clientes>> PorCedulaAsync(Clientes entidad)
        {
            if (entidad == null || string.IsNullOrEmpty(entidad.Cedula))
                throw new Exception("lbFaltaInformacion");

            return await _conexion.Clientes!
                .Where(x => x.Cedula!.Contains(entidad.Cedula!))
                .ToListAsync();
        }

    }
}


