using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Repositories;

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

        public async Task<Servicio> GetByIdAsync(int servicio_id)
        {
            Servicio unServicio = await _servicioRepository
                .GetByIdAsync(servicio_id);

            if (unServicio.Id == 0)
                throw new AppValidationException($"Servicio no encontrado con el ID {servicio_id}");

            return unServicio;
        }
    }
}
