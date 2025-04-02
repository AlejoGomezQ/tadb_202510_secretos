using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces
{
    public interface IServicioRepository
    {
        public Task<List<Servicio>> GetAllAsync();

        public Task<Servicio> GetByGuidAsync(Guid servicio_id);

        public Task<Servicio> GetByNameAsync(string servicio_nombre);

        public Task<List<Componente>> GetAssociatedComponentsAsync(Guid servicio_id);

        public Task<List<Consumo>> GetAssociatedConsumptionAsync(Guid servicio_id);

        public Task<int> GetTotalComponentsByServiceGuidAsync(Guid servicio_id);

        public Task<int> GetTotalConsumptionByServiceGuidAsync(Guid servicio_id);

        public Task<bool> CreateAsync(Servicio unServicio);

        public Task<bool> UpdateAsync(Servicio unServicio);

        public Task<bool> RemoveAsync(Guid servicio_id);
    }
}
