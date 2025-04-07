using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumosController(ConsumoService consumoService) : Controller
    {
        private readonly ConsumoService _consumoService = consumoService;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var losConsumos = await _consumoService
                .GetAllAsync();

            return Ok(losConsumos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Consumo unConsumo)
        {
            try
            {
                var consumoCreado = await _consumoService
                    .CreateAsync(unConsumo);

                return Ok(consumoCreado);
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
        public async Task<IActionResult> UpdateAsync(Consumo unConsumo)
        {
            try
            {
                var consumoActualizado = await _consumoService
                    .UpdateAsync(unConsumo);

                return Ok(consumoActualizado);
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

        [HttpDelete]
        public async Task<IActionResult> RemoveAsync(Consumo unConsumo)
        {
            try
            {
                var consumoEliminado = await _consumoService
                    .RemoveAsync(unConsumo);

                return Ok($"El consumo para {consumoEliminado} fue eliminado correctamente!");
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
