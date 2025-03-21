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
    }
}
