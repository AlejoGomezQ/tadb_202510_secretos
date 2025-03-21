using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces
{
    public interface IServicioRepository
    {
        public Task<List<Servicio>> GetAllAsync();

        public Task<Servicio> GetByGuidAsync(Guid servicio_id);

        public Task<List<Componente>> GetAssociatedComponentsAsync(Guid servicio_id);
    }
}
