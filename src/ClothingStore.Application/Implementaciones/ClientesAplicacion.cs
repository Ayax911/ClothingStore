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

            if (!string.IsNullOrEmpty(entidad.Email))
            {
                var emailExiste = await _conexion.Clientes
                    .AnyAsync(c => c.Email == entidad.Email);
                
                if (emailExiste)
                    throw new InvalidOperationException("El email ya está registrado");
            }
            
            // Validar cédula duplicada
            if (!string.IsNullOrEmpty(entidad.Cedula))
            {
                var cedulaExiste = await _conexion.Clientes
                    .AnyAsync(c => c.Cedula == entidad.Cedula);
                
                if (cedulaExiste)
                    throw new InvalidOperationException("La cédula ya está registrada");
            }
            
            // Establecer fecha de registro
            entidad.FechaRegistro = DateTime.UtcNow;

            _conexion.Clientes!.Add(entidad);
            await _conexion.SaveChangesAsync();
            return entidad;
        }

        public async Task<Clientes?> ModificarAsync(Clientes entidad)
        {

            var existente = await _conexion.Clientes.FindAsync(entidad.Id);
            if (existente == null)
                throw new KeyNotFoundException("Cliente no encontrado");

            // Validar email duplicado (si se está cambiando)
            if (!string.IsNullOrEmpty(entidad.Email) && 
                entidad.Email != existente.Email)
            {
                var emailExiste = await _conexion.Clientes
                    .AnyAsync(c => c.Email == entidad.Email && c.Id != entidad.Id);
                
                if (emailExiste)
                    throw new InvalidOperationException("El email ya está registrado por otro cliente");
            }
            
            // Validar cédula duplicada (si se está cambiando)
            if (!string.IsNullOrEmpty(entidad.Cedula) && 
                entidad.Cedula != existente.Cedula)
            {
                var cedulaExiste = await _conexion.Clientes
                    .AnyAsync(c => c.Cedula == entidad.Cedula && c.Id != entidad.Id);
                
                if (cedulaExiste)
                    throw new InvalidOperationException("La cédula ya está registrada por otro cliente");
            }
            
            existente.Nombre = entidad.Nombre;
            existente.Telefono = entidad.Telefono;
            existente.Cedula = entidad.Cedula;
            existente.Email = entidad.Email;
            
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


