using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces
{
    public interface IServicioRepository
    {
        public Task<List<Servicio>> GetAllAsync();

        public Task<Servicio> GetByIdAsync(string servicio_id);

        public Task<Servicio> GetByNameAsync(string servicio_nombre);

        public Task<List<Componente>> GetAssociatedComponentsAsync(string servicio_id);

        public Task<List<Consumo>> GetAssociatedConsumptionAsync(string servicio_id);

        public Task<int> GetTotalComponentsByServiceIdAsync(string servicio_id);

        public Task<int> GetTotalConsumptionByServiceIdAsync(string servicio_id);

        public Task<bool> CreateAsync(Servicio unServicio);

        public Task<bool> UpdateAsync(Servicio unServicio);

        public Task<bool> RemoveAsync(string servicio_id);
    }
}
