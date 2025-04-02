using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Repositories;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Services
{
    public class PeriodoService(IPeriodoRepository periodoRepository)
    {
        private readonly IPeriodoRepository _periodoRepository = periodoRepository;

        public async Task<List<Periodo>> GetAllAsync()
        {
            return await _periodoRepository
                .GetAllAsync();
        }

        public async Task<Periodo> GetByGuidAsync(Guid periodo_id)
        {
            Periodo unPeriodo = await _periodoRepository
                .GetByGuidAsync(periodo_id);

            if (unPeriodo.Id == Guid.Empty)
                throw new AppValidationException($"Periodo no encontrado con el Guid {periodo_id}");

            return unPeriodo;
        }

        public async Task<List<Consumo>> GetAssociatedConsumptionAsync(Guid periodo_id)
        {
            Periodo unPeriodo = await _periodoRepository
                .GetByGuidAsync(periodo_id);

            if (unPeriodo.Id == Guid.Empty)
                throw new AppValidationException($"Periodo no encontrado con el Guid {periodo_id}");

            var consumosAsociados = await _periodoRepository
                .GetAssociatedConsumptionAsync(periodo_id);

            if (consumosAsociados.Count == 0)
                throw new AppValidationException($"Periodo {unPeriodo.MesFacturacion} no tiene consumos asociados");

            return consumosAsociados;
        }

        //TODO: Crear el método para insertar - Periodo
        //TODO: Crear el método para actualiza - Periodo
        //TODO: Crear el método para borrar - Periodo
    }
}
