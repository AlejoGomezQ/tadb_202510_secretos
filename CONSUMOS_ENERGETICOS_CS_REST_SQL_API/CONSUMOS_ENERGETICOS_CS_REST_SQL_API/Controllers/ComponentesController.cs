using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentesController(ComponenteService componenteService) : Controller
    {
        private readonly ComponenteService _componenteService = componenteService;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var losPeriodos = await _componenteService
                .GetAllAsync();

            return Ok(losPeriodos);
        }

        [HttpGet("{componente_id:Guid}")]
        public async Task<IActionResult> GetByGuidAsync(Guid componente_id)
        {
            try
            {
                var unComponente = await _componenteService
                    .GetByGuidAsync(componente_id);

                return Ok(unComponente);
            }
            catch (AppValidationException error)
            {
                return NotFound(error.Message);
            }
        }

    }
}
