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

        //TODO: Obtener por GUID - Consumo
        //TODO: Crear el método para mapear el HTTP - POST - Consumo
        //TODO: Crear el método para mapear el HTTP - PUT - Consumo
        //TODO: Crear el método para mapear el HTTP - DEL - Consumo

    }
}
