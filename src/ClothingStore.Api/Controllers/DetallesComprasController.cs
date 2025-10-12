using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClothingStore.Application.Interfaces;
using ClothingStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DetallesComprasController : ControllerBase
    {
        private readonly IDetallesComprasAplicacion _detallesComprasAplicacion;
        private readonly ILogger<DetallesComprasController> _logger;

        public DetallesComprasController(
            IDetallesComprasAplicacion detallesComprasAplicacion,
            ILogger<DetallesComprasController> logger)
        {
            _detallesComprasAplicacion = detallesComprasAplicacion;
            _logger = logger;
        }

        // GET: api/detallescompras
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var detalles = await _detallesComprasAplicacion.ListarAsync();
                return Ok(detalles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar detalles de compras");
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener la lista de detalles de compras",
                    detalle = ex.Message
                });
            }
        }

        // GET: api/detallescompras/compra/{compraId}
        [HttpGet("compra/{compraId:int}")]
        public async Task<IActionResult> ObtenerPorCompra(int compraId)
        {
            try
            {
                if (compraId <= 0)
                    return BadRequest(new { mensaje = "El ID de compra debe ser mayor a 0" });

                var detalles = await _detallesComprasAplicacion.PorCompraAsync(compraId);

                if (detalles == null || !detalles.Any())
                    return NotFound(new { mensaje = $"No se encontraron detalles para la compra ID {compraId}" });

                return Ok(detalles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar detalles por compra ID: {CompraId}", compraId);
                return StatusCode(500, new
                {
                    mensaje = "Error al buscar los detalles de la compra",
                    detalle = ex.Message
                });
            }
        }

        // POST: api/detallescompras
        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] DetallesCompras detalle)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var nuevo = await _detallesComprasAplicacion.GuardarAsync(detalle);
                return CreatedAtAction(
                    nameof(ObtenerPorCompra),
                    new { compraId = nuevo!.CompraId },
                    nuevo
                );
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Intento de guardar detalle de compra nulo");
                return BadRequest(new { mensaje = "Debe proporcionar los datos del detalle de compra" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Detalle de compra ya existe con ID: {Id}", detalle?.Id);
                return Conflict(new
                {
                    mensaje = "El detalle de compra ya existe en la base de datos",
                    detalle = ex.Message
                });
            }
            catch (Exception ex) when (ex.Message.Contains("lbFKInvalida"))
            {
                _logger.LogWarning(ex, "Intento de guardar detalle con CompraId o ProductoId inválido");
                return BadRequest(new
                {
                    mensaje = "La compra o el producto especificado no existen",
                    detalle = "Verifique que CompraId y ProductoId sean válidos"
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al guardar detalle de compra");
                return StatusCode(500, new
                {
                    mensaje = "Error al guardar el detalle de compra en la base de datos",
                    detalle = "Verifique que los datos sean válidos y que la compra y producto existan"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al guardar detalle de compra");
                return StatusCode(500, new
                {
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message
                });
            }
        }

        // PUT: api/detallescompras/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] DetallesCompras detalle)
        {
            try
            {
                if (id != detalle.Id)
                    return BadRequest(new { mensaje = "El ID del detalle no coincide con la URL" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var actualizado = await _detallesComprasAplicacion.ModificarAsync(detalle);

                if (actualizado == null)
                    return NotFound(new { mensaje = $"No se encontró el detalle de compra con ID {id}" });

                return Ok(actualizado);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Intento de modificar detalle de compra nulo");
                return BadRequest(new { mensaje = "Debe proporcionar los datos del detalle de compra" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Detalle de compra no existe con ID: {Id}", id);
                return NotFound(new
                {
                    mensaje = $"El detalle de compra con ID {id} no existe",
                    detalle = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Detalle de compra no encontrado: {Id}", id);
                return NotFound(new { mensaje = ex.Message });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error de concurrencia al modificar detalle de compra ID: {Id}", id);
                return Conflict(new
                {
                    mensaje = "El detalle de compra fue modificado por otro usuario",
                    detalle = "Por favor, recargue los datos e intente nuevamente"
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al modificar detalle de compra");
                return StatusCode(500, new
                {
                    mensaje = "Error al actualizar el detalle de compra",
                    detalle = "Verifique que los datos sean válidos"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al modificar detalle de compra ID: {Id}", id);
                return StatusCode(500, new
                {
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message
                });
            }
        }

        // DELETE: api/detallescompras/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Borrar(int id)
        {
            try
            {
                var detalle = new DetallesCompras { Id = id };
                var eliminado = await _detallesComprasAplicacion.BorrarAsync(detalle);

                if (eliminado == null)
                    return NotFound(new { mensaje = $"No se encontró el detalle de compra con ID {id}" });

                return NoContent();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Intento de borrar detalle de compra nulo");
                return BadRequest(new { mensaje = "ID de detalle de compra inválido" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Detalle de compra no existe con ID: {Id}", id);
                return NotFound(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Detalle de compra no encontrado para eliminar: {Id}", id);
                return NotFound(new { mensaje = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al eliminar detalle de compra ID: {Id}", id);
                return StatusCode(500, new
                {
                    mensaje = "No se pudo eliminar el detalle de compra",
                    detalle = "Puede haber datos relacionados que impidan la eliminación"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar detalle de compra ID: {Id}", id);
                return StatusCode(500, new
                {
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message
                });
            }
        }
    }
}