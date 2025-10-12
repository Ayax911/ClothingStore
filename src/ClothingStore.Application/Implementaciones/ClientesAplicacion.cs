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

            var existente = await _conexion.Clientes.FindAsync(entidad.Id);
            if (existente == null)
                throw new KeyNotFoundException("Cliente no encontrado");
            
            existente.Nombre = entidad.Nombre;
            existente.Telefono = entidad.Telefono;
            existente.Cedula = entidad.Cedula;
            
            await _conexion.SaveChangesAsync();
            return existente;
     
        }

        public async Task<Clientes?> BorrarAsync(Clientes entidad)
        {
        
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id == 0)
                throw new Exception("lbNoSeGuardo");

            var existente = await _conexion.Clientes.FindAsync(entidad.Id);

            if (existente == null)
                return null;

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
                .Where(x => x.Cedula! == entidad.Cedula)
                .ToListAsync();
        }

    }
}


