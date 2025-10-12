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
    public class ProductosController : ControllerBase
    {

        private readonly IProductosAplicacion _productosAplicacion;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductosAplicacion productosAplicacion, ILogger<ProductosController> logger)
        {
            _productosAplicacion = productosAplicacion;
            _logger = logger;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var productos = await _productosAplicacion.ListarAsync();
                return Ok(productos);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar productos");
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener la lista de productos",
                    detalle = ex.Message
                });

            }

        }

        // GET: api/productos/{nombre}
        [HttpGet("{nombre}")]
        public async Task<IActionResult> ObtenerPorNombre(string nombre)
        {
            try
            {

                var productos = await _productosAplicacion.PorNombreAsync(nombre);

                if (productos == null || !productos.Any())
                    return NotFound();

                return Ok(productos);

            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Nombre nula en búsqueda");
                return BadRequest(new { mensaje = "La nombre no puede estar vacía" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar producto por nombre: {nombre}", nombre);
                return StatusCode(500, new
                {
                    mensaje = "Error al buscar el producto",
                    detalle = ex.Message
                });
            }

        }

        // POST: api/productos
        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Productos producto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var nuevo = await _productosAplicacion.GuardarAsync(producto);
                return CreatedAtAction(nameof(ObtenerPorNombre), new { nombre = nuevo!.Nombre }, nuevo);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Intento de guardar producto nulo");
                return BadRequest(new { mensaje = "Debe proporcionar los datos del producto" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "producto ya existe con ID: {Id}", producto?.Id);
                return Conflict(new
                {
                    mensaje = "El producto ya existe en la base de datos",
                    detalle = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al guardar producto");
                return StatusCode(500, new
                {
                    mensaje = "Error al guardar el producto en la base de datos",
                    detalle = "Verifique que los datos sean válidos"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al guardar producto");
                return StatusCode(500, new
                {
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message
                });
            }
        }

        // PUT: api/productos/{nombre}
        [HttpPut("{nombre:string}")]
        public async Task<IActionResult> Modificar(string nombre, [FromBody] Productos producto)
        {
            try
            {
                if (nombre != producto.Nombre)
                    return BadRequest("El nombre del producto no coincide con la URL.");

                var actualizado = await _productosAplicacion.ModificarAsync(producto);
                if (actualizado == null)
                    return NotFound();

                return Ok(actualizado);

            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Intento de modificar producto nulo");
                return BadRequest(new { mensaje = "Debe proporcionar los datos del producto" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "producto no existe con nombre: {Nombre}", nombre);
                return NotFound(new
                {
                    mensaje = $"El producto con Nombre {nombre} no existe",
                    detalle = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "producto no encontrado: {Nombre}", nombre);
                return NotFound(new { mensaje = ex.Message });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error de concurrencia al modificar producto Nombre: {Nombre}", nombre);
                return Conflict(new
                {
                    mensaje = "El producto fue modificado por otro usuario",
                    detalle = "Por favor, recargue los datos e intente nuevamente"
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al modificar producto");
                return StatusCode(500, new
                {
                    mensaje = "Error al actualizar el producto",
                    detalle = "Verifique que los datos sean válidos"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al modificar producto Nombre: {Nombre}", nombre);
                return StatusCode(500, new
                {
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message
                });
            }

        }
        
        // DELETE: api/productos/{nombre}
        [HttpDelete("{nombre:string}")]
        public async Task<IActionResult> Borrar(string nombre)
        {
            try
            {
                var producto = new Productos { Nombre = nombre };

                // La pasamos al método que espera una entidad
                var eliminado = await _productosAplicacion.BorrarAsync(producto);

                if (eliminado == null)
                    return NotFound(new { mensaje = $"No se encontró el Producto con Nombre {nombre}" });

                return NoContent();
                
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Intento de borrar Producto nulo");
                return BadRequest(new { mensaje = "Nombre del producto inválido" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Producto no existe con Nombre: {Nombre}", nombre);
                return NotFound(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Producto no encontrado para eliminar: {Nombre}", nombre);
                return NotFound(new { mensaje = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al eliminar Producto Nombre: {Nombre}", nombre);
                return StatusCode(500, new { 
                    mensaje = "No se pudo eliminar el Producto",
                    detalle = "El Producto puede tener datos relacionados (compras, etc.)" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar Producto Nombre: {Nombre}", nombre);
                return StatusCode(500, new { 
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message 
                });
            }
        
                 
        }

    }
}
