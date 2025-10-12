using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClothingStore.Application.Interfaces;
using ClothingStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;



namespace ClothingStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // JWT: el middleware valida el token antes de entrar aquí
    public class ClientesController : ControllerBase
    {
        private readonly IClientesAplicacion _clientesAplicacion;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IClientesAplicacion clientesAplicacion,  ILogger<ClientesController> logger)
        {
            _clientesAplicacion = clientesAplicacion;
            _logger = logger;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var clientes = await _clientesAplicacion.ListarAsync();
                 return Ok(clientes);

            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error al listar clientes");
                return StatusCode(500, new { 
                    mensaje = "Error al obtener la lista de clientes",
                    detalle = ex.Message 
                });
                
            }
            
        }

        // GET: api/clientes/{cedula}
        [HttpGet("{cedula}")]
        public async Task<IActionResult> ObtenerPorCedula(string cedula)
        {
            try
            {
                var entidad = new Clientes { Cedula = cedula };
                var clientes = await _clientesAplicacion.PorCedulaAsync(entidad);

                if (clientes == null || !clientes.Any())
                    return NotFound();

                return Ok(clientes);

            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Cédula nula en búsqueda");
                return BadRequest(new { mensaje = "La cédula no puede estar vacía" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar cliente por cédula: {Cedula}", cedula);
                return StatusCode(500, new
                {
                    mensaje = "Error al buscar el cliente",
                    detalle = ex.Message
                });
            }
            
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Clientes cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
                var nuevo = await _clientesAplicacion.GuardarAsync(cliente);
                return CreatedAtAction(nameof(ObtenerPorCedula), new { cedula = nuevo!.Cedula }, nuevo);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Intento de guardar cliente nulo");
                return BadRequest(new { mensaje = "Debe proporcionar los datos del cliente" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cliente ya existe con ID: {Id}", cliente?.Id);
                return Conflict(new { 
                    mensaje = "El cliente ya existe en la base de datos",
                    detalle = ex.Message 
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al guardar cliente");
                return StatusCode(500, new { 
                    mensaje = "Error al guardar el cliente en la base de datos",
                    detalle = "Verifique que los datos sean válidos" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al guardar cliente");
                return StatusCode(500, new { 
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message 
                });
            }
        }

        // PUT: api/clientes/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] Clientes cliente)
        {
            try
            {
                if (id != cliente.Id)
                    return BadRequest("El ID del cliente no coincide con la URL.");

                var actualizado = await _clientesAplicacion.ModificarAsync(cliente);
                if (actualizado == null)
                    return NotFound();

                return Ok(actualizado);

            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Intento de modificar cliente nulo");
                return BadRequest(new { mensaje = "Debe proporcionar los datos del cliente" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cliente no existe con ID: {Id}", id);
                return NotFound(new { 
                    mensaje = $"El cliente con ID {id} no existe",
                    detalle = ex.Message 
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Cliente no encontrado: {Id}", id);
                return NotFound(new { mensaje = ex.Message });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error de concurrencia al modificar cliente ID: {Id}", id);
                return Conflict(new { 
                    mensaje = "El cliente fue modificado por otro usuario",
                    detalle = "Por favor, recargue los datos e intente nuevamente" 
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al modificar cliente");
                return StatusCode(500, new { 
                    mensaje = "Error al actualizar el cliente",
                    detalle = "Verifique que los datos sean válidos" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al modificar cliente ID: {Id}", id);
                return StatusCode(500, new { 
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message 
                });
            }
            
        }

        // DELETE: api/clientes/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Borrar(int id)
        {
            try
            {
                var cliente = new Clientes { Id = id };

                // La pasamos al método que espera una entidad
                var eliminado = await _clientesAplicacion.BorrarAsync(cliente);

                if (eliminado == null)
                    return NotFound(new { mensaje = $"No se encontró el cliente con ID {id}" });

                return NoContent();

            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Intento de borrar cliente nulo");
                return BadRequest(new { mensaje = "ID de cliente inválido" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cliente no existe con ID: {Id}", id);
                return NotFound(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Cliente no encontrado para eliminar: {Id}", id);
                return NotFound(new { mensaje = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al eliminar cliente ID: {Id}", id);
                return StatusCode(500, new
                {
                    mensaje = "No se pudo eliminar el cliente",
                    detalle = "El cliente puede tener datos relacionados (compras, etc.)"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar cliente ID: {Id}", id);
                return StatusCode(500, new
                {
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message
                });
            }
        }
        
    }
}
