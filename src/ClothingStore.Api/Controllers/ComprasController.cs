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
    public class ComprasController : ControllerBase
    {
        private readonly IComprasAplicacion _comprasAplicacion;
        private readonly ILogger<ComprasController> _logger;

        public ComprasController(
            IComprasAplicacion comprasAplicacion,
            ILogger<ComprasController> logger)
        {
            _comprasAplicacion = comprasAplicacion;
            _logger = logger;
        }

        // GET: api/compras
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var compras = await _comprasAplicacion.ListarAsync();
                return Ok(compras);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar compras");
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener la lista de compras",
                    detalle = ex.Message
                });
            }
        }

        // GET: api/compras/{codigo}
        [HttpGet("{codigo}")]
        public async Task<IActionResult> ObtenerPorCodigo(string codigo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigo))
                    return BadRequest(new { mensaje = "El código es obligatorio" });

                var compras = await _comprasAplicacion.PorCodigoAsync(codigo);

                if (compras == null || !compras.Any())
                    return NotFound(new { mensaje = $"No se encontraron compras con código {codigo}" });

                return Ok(compras);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Código nulo en búsqueda");
                return BadRequest(new { mensaje = "El código no puede estar vacío" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar compra por código: {Codigo}", codigo);
                return StatusCode(500, new
                {
                    mensaje = "Error al buscar la compra",
                    detalle = ex.Message
                });
            }
        }

        // POST: api/compras
        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Compras compra)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var nueva = await _comprasAplicacion.GuardarAsync(compra);
                return CreatedAtAction(
                    nameof(ObtenerPorCodigo),
                    new { codigo = nueva!.Codigo },
                    nueva
                );
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Intento de guardar compra nula");
                return BadRequest(new { mensaje = "Debe proporcionar los datos de la compra" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Compra ya existe con ID: {Id}", compra?.Id);
                return Conflict(new
                {
                    mensaje = "La compra ya existe en la base de datos",
                    detalle = ex.Message
                });
            }
            catch (Exception ex) when (ex.Message.Contains("lbCodigoExistente"))
            {
                _logger.LogWarning(ex, "Intento de guardar compra con código duplicado: {Codigo}", compra?.Codigo);
                return Conflict(new
                {
                    mensaje = "Ya existe una compra con ese código",
                    detalle = $"El código '{compra?.Codigo}' ya está registrado en el sistema"
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al guardar compra");
                return StatusCode(500, new
                {
                    mensaje = "Error al guardar la compra en la base de datos",
                    detalle = "Verifique que los datos sean válidos y que el cliente exista"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al guardar compra");
                return StatusCode(500, new
                {
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message
                });
            }
        }

        // PUT: api/compras/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] Compras compra)
        {
            try
            {
                if (id != compra.Id)
                    return BadRequest(new { mensaje = "El ID de la compra no coincide con la URL" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var actualizada = await _comprasAplicacion.ModificarAsync(compra);

                if (actualizada == null)
                    return NotFound(new { mensaje = $"No se encontró la compra con ID {id}" });

                return Ok(actualizada);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Intento de modificar compra nula");
                return BadRequest(new { mensaje = "Debe proporcionar los datos de la compra" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Compra no existe con ID: {Id}", id);
                return NotFound(new
                {
                    mensaje = $"La compra con ID {id} no existe",
                    detalle = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Compra no encontrada: {Id}", id);
                return NotFound(new { mensaje = ex.Message });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error de concurrencia al modificar compra ID: {Id}", id);
                return Conflict(new
                {
                    mensaje = "La compra fue modificada por otro usuario",
                    detalle = "Por favor, recargue los datos e intente nuevamente"
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al modificar compra");
                return StatusCode(500, new
                {
                    mensaje = "Error al actualizar la compra",
                    detalle = "Verifique que los datos sean válidos"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al modificar compra ID: {Id}", id);
                return StatusCode(500, new
                {
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message
                });
            }
        }

        // DELETE: api/compras/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Borrar(int id)
        {
            try
            {
                var compra = new Compras { Id = id };
                var eliminada = await _comprasAplicacion.BorrarAsync(compra);

                if (eliminada == null)
                    return NotFound(new { mensaje = $"No se encontró la compra con ID {id}" });

                return NoContent();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Intento de borrar compra nula");
                return BadRequest(new { mensaje = "ID de compra inválido" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Compra no existe con ID: {Id}", id);
                return NotFound(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Compra no encontrada para eliminar: {Id}", id);
                return NotFound(new { mensaje = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al eliminar compra ID: {Id}", id);
                return StatusCode(500, new
                {
                    mensaje = "No se pudo eliminar la compra",
                    detalle = "La compra puede tener detalles de compra relacionados. Elimine los detalles primero."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar compra ID: {Id}", id);
                return StatusCode(500, new
                {
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message
                });
            }
        }
    }
}