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
    }
}
