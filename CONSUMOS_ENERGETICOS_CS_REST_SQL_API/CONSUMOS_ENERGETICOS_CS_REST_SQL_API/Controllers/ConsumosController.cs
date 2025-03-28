using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
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

        //TODO: Obtener Consumo por Guid de Periodo
        [HttpGet("Periodos/{periodo_id:Guid}")]
        public async Task<IActionResult> GetByTermGuidAsync(Guid periodo_id)
        {
            try
            {
                var losConsumosDelPeriodo = await _consumoService
                    .GetByTermGuidAsync(periodo_id);

                return Ok(losConsumosDelPeriodo);
            }
            catch (AppValidationException error)
            {
                return NotFound(error.Message);
            }
        }

        //TODO: Crear el método para mapear el HTTP - GET Consumo por Guid de Servicio
        //TODO: Crear el método para mapear el HTTP - POST - Consumo
        //TODO: Crear el método para mapear el HTTP - PUT - Consumo
        //TODO: Crear el método para mapear el HTTP - DEL - Consumo

    }
}
