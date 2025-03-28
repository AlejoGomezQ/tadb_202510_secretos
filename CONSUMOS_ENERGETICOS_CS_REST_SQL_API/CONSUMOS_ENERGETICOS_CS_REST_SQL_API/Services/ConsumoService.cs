using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Services
{
    public class ConsumoService(IConsumoRepository consumoRepository)
    {
        private readonly IConsumoRepository _consumoRepository = consumoRepository;

        public async Task<List<Consumo>> GetAllAsync()
        {
            return await _consumoRepository
                .GetAllAsync();
        }

        //TODO: Crear el método para consultar por Guid - Consumo
        //TODO: Crear el método para insertar - Consumo
        //TODO: Crear el método para actualiza - Consumo
        //TODO: Crear el método para borrar - Consumo

    }
}
