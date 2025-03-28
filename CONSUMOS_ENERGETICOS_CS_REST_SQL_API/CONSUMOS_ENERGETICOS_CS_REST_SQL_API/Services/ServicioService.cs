using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Services
{
    public class ServicioService(IServicioRepository servicioRepository)
    {
        private readonly IServicioRepository _servicioRepository = servicioRepository;

        public async Task<List<Servicio>> GetAllAsync()
        {
            return await _servicioRepository
            .GetAllAsync();
        }

        public async Task<Servicio> GetByGuidAsync(Guid servicio_id)
        {
            Servicio unServicio = await _servicioRepository
                .GetByGuidAsync(servicio_id);

            if (unServicio.Id == Guid.Empty)
                throw new AppValidationException($"Servicio no encontrado con el ID {servicio_id}");

            return unServicio;
        }

        public async Task<List<Componente>> GetAssociatedComponentsAsync(Guid servicio_id)
        {
            Servicio unServicio = await _servicioRepository
                .GetByGuidAsync(servicio_id);

            if (unServicio.Id == Guid.Empty)
                throw new AppValidationException($"Servicio no encontrado con el id {servicio_id}");

            var componentesAsociados = await _servicioRepository
                .GetAssociatedComponentsAsync(servicio_id);

            if (componentesAsociados.Count == 0)
                throw new AppValidationException($"Servicio {unServicio.Nombre} no tiene componentes asociados");

            return componentesAsociados;
        }

        //TODO: Crear el método para insertar - Servicio
        //TODO: Crear el método para actualiza - Servicio
        //TODO: Crear el método para borrar - Servicio
    }
}
