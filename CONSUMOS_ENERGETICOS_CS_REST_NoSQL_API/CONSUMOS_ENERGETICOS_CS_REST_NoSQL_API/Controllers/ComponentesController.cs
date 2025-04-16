using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentesController(ComponenteService componenteService) : Controller
    {
        private readonly ComponenteService _componenteService = componenteService;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var losComponentes = await _componenteService
                .GetAllAsync();

            return Ok(losComponentes);
        }

        [HttpGet("{componente_id:length(24)}")]
        public async Task<IActionResult> GetByIdAsync(string componente_id)
        {
            try
            {
                var unComponente = await _componenteService
                    .GetByIdAsync(componente_id);

                return Ok(unComponente);
            }
            catch (AppValidationException error)
            {
                return NotFound(error.Message);
            }
        }

        //TODO: Crear el método para mapear el HTTP - GET - Componentes por Periodo

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Componente unComponente)
        {
            try
            {
                var componenteCreado = await _componenteService
                    .CreateAsync(unComponente);

                return Ok(componenteCreado);
            }
            catch (AppValidationException error)
            {
                return BadRequest($"Error de validación: {error.Message}");
            }
            catch (DbOperationException error)
            {
                return BadRequest($"Error de operacion en DB: {error.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(Componente unComponente)
        {
            try
            {
                var componenteActualizado = await _componenteService
                    .UpdateAsync(unComponente);

                return Ok(componenteActualizado);
            }
            catch (AppValidationException error)
            {
                return BadRequest($"Error de validación: {error.Message}");
            }
            catch (DbOperationException error)
            {
                return BadRequest($"Error de operacion en DB: {error.Message}");
            }
        }

        [HttpDelete("{componente_id:length(24)}")]
        public async Task<IActionResult> RemoveAsync(string componente_id)
        {
            try
            {
                var nombreServicioBorrado = await _componenteService
                    .RemoveAsync(componente_id);

                return Ok($"El servicio {nombreServicioBorrado} fue eliminado correctamente!");
            }
            catch (AppValidationException error)
            {
                return BadRequest($"Error de validación: {error.Message}");
            }
            catch (DbOperationException error)
            {
                return BadRequest($"Error de operacion en DB: {error.Message}");
            }
        }
    }
}
