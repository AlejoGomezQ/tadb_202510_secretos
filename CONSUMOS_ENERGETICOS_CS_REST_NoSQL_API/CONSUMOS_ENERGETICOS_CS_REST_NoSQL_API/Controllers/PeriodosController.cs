using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeriodosController(PeriodoService periodoService) : Controller
    {
        private readonly PeriodoService _periodoService = periodoService;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var losPeriodos = await _periodoService
                .GetAllAsync();

            return Ok(losPeriodos);
        }

        [HttpGet("{periodo_id:length(24)}")]
        public async Task<IActionResult> GetByIdAsync(string periodo_id)
        {
            try
            {
                var unPeriodo = await _periodoService
                    .GetByIdAsync(periodo_id);

                return Ok(unPeriodo);
            }
            catch (AppValidationException error)
            {
                return NotFound(error.Message);
            }
        }

        [HttpGet("{periodo_id:length(24)}/Consumos")]
        public async Task<IActionResult> GetAssociatedConsumptionAsync(string periodo_id)
        {
            try
            {
                var losConsumosAsociados = await _periodoService
                    .GetAssociatedConsumptionAsync(periodo_id);

                return Ok(losConsumosAsociados);
            }
            catch (AppValidationException error)
            {
                return NotFound(error.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Periodo unPeriodo)
        {
            try
            {
                var periodoCreado = await _periodoService
                    .CreateAsync(unPeriodo);

                return Ok(periodoCreado);
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
        public async Task<IActionResult> UpdateAsync(Periodo unPeriodo)
        {
            try
            {
                var periodoActualizado = await _periodoService
                    .UpdateAsync(unPeriodo);

                return Ok(periodoActualizado);
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

        [HttpDelete("{periodo_id:length(24)}")]
        public async Task<IActionResult> RemoveAsync(string periodo_id)
        {
            try
            {
                var mesFacturacionEliminado = await _periodoService
                    .RemoveAsync(periodo_id);

                return Ok($"El periodo {mesFacturacionEliminado} fue eliminado correctamente!");
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
