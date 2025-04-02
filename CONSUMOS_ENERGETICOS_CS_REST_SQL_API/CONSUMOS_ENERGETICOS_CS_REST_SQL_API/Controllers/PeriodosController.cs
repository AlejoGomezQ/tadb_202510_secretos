using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Controllers
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

        [HttpGet("{periodo_id:Guid}")]
        public async Task<IActionResult> GetByGuidAsync(Guid periodo_id)
        {
            try
            {
                var unPeriodo = await _periodoService
                    .GetByGuidAsync(periodo_id);

                return Ok(unPeriodo);
            }
            catch (AppValidationException error)
            {
                return NotFound(error.Message);
            }
        }

        [HttpGet("{periodo_id:Guid}/Consumos")]
        public async Task<IActionResult> GetAssociatedConsumptionAsync(Guid periodo_id)
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

        //TODO: Crear el método para mapear el HTTP - GET - Consumos por Periodo
        //TODO: Crear el método para mapear el HTTP - POST - Periodo
        //TODO: Crear el método para mapear el HTTP - PUT - Periodo
        //TODO: Crear el método para mapear el HTTP - DEL - Periodo
    }
}
