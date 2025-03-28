using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Services
{
    public class ConsumoService(IConsumoRepository consumoRepository,
                                IPeriodoRepository periodoRepository)
    {
        private readonly IConsumoRepository _consumoRepository = consumoRepository;
        private readonly IPeriodoRepository _periodoRepository = periodoRepository;

        public async Task<List<Consumo>> GetAllAsync()
        {
            return await _consumoRepository
                .GetAllAsync();
        }

        public async Task<List<Consumo>> GetByTermGuidAsync(Guid periodo_id)
        {
            Periodo unPeriodo = await _periodoRepository
                .GetByGuidAsync(periodo_id);

            if (unPeriodo.Id == Guid.Empty)
                throw new AppValidationException($"Periodo no encontrado con el id {periodo_id}");

            var consumosAsociados = await _consumoRepository
                .GetByTermGuidAsync(periodo_id);

            if (consumosAsociados.Count == 0)
                throw new AppValidationException($"Periodo {unPeriodo.MesFacturacion} no tiene consumos asociados");

            return consumosAsociados;
        }

        //TODO: Consultar Consumo por Guid de Servicio
        //TODO: Crear el método para insertar - Consumo
        //TODO: Crear el método para actualiza - Consumo
        //TODO: Crear el método para borrar - Consumo
    }
}
