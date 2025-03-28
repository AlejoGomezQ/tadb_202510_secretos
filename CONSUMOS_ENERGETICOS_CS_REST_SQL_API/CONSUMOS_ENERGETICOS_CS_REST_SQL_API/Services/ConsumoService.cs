using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Services
{
    public class ConsumoService(IConsumoRepository consumoRepository,
                                IPeriodoRepository periodoRepository,
                                IServicioRepository servicioRepository)
    {
        private readonly IConsumoRepository _consumoRepository = consumoRepository;
        private readonly IPeriodoRepository _periodoRepository = periodoRepository;
        private readonly IServicioRepository _servicioRepository = servicioRepository;

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

        public async Task<List<Consumo>> GetByServiceGuidAsync(Guid servicio_id)
        {
            Servicio unServicio = await _servicioRepository
                .GetByGuidAsync(servicio_id);

            if (unServicio.Id == Guid.Empty)
                throw new AppValidationException($"Servicio no encontrado con el id {servicio_id}");

            var consumosAsociados = await _consumoRepository
                .GetByServiceGuidAsync(servicio_id);

            if (consumosAsociados.Count == 0)
                throw new AppValidationException($"Servicio {unServicio.Nombre} no tiene consumos asociados");

            return consumosAsociados;
        }

        //TODO: Crear el método para insertar - Consumo
        //TODO: Crear el método para actualiza - Consumo
        //TODO: Crear el método para borrar - Consumo
    }
}
